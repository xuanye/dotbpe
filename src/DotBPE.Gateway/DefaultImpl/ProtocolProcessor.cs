using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc;
using DotBPE.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace DotBPE.Gateway
{
    /// <inheritdoc />
    public class ProtocolProcessor:IProtocolProcessor
    {
        private readonly IJsonParser _jsonParser;
        private readonly IHttpMetricFactory _metricFactory;
        private readonly ILogger _logger;
        private readonly HttpRouteOptions _routeOptions;
        private readonly IEnumerable<IAdditionalHttpParser> _parsers;
        private readonly RpcGatewayOptions _gatewayOptions;
        private static readonly ConcurrentDictionary<string, RouteItem> ROUTER_CACHE
            = new ConcurrentDictionary<string, RouteItem>();

        static readonly Dictionary<string,RestfulVerb> VerbsCache = new Dictionary<string, RestfulVerb>()
        {
            {"get", RestfulVerb.Get},
            {"post", RestfulVerb.Post},
            {"put", RestfulVerb.Put},
            {"patch", RestfulVerb.Patch},
            {"delete", RestfulVerb.Delete}
        };

        public ProtocolProcessor(
            IJsonParser jsonParser,
            IHttpMetricFactory metricFactory,
            HttpRouteOptions routeOptions,
            IEnumerable<IAdditionalHttpParser> parsers,
            RpcGatewayOptions gatewayOptions,
            ILogger logger)
        {
            this._jsonParser = jsonParser;
            this._metricFactory = metricFactory;
            this._logger = logger;
            this._routeOptions = routeOptions;
            this._parsers = parsers;
            this._gatewayOptions = gatewayOptions;
        }



        public async Task<bool> Invoke(HttpContext context)
        {
            bool result;
            using (var metric = this._metricFactory.Create())
            {
                await metric.AddToMetricsAsync(context);
                result = await InvokeInner(context);
            }

            return result;
        }

        private async Task<bool> InvokeInner(HttpContext context)
        {
            var req = context.Request;
            var res = context.Response;

            bool hasRes;
            var router = FindRouter(req);

            if (router == null) //not match any router
            {
                this._logger.LogWarning("[{0}:{1}] does not match any router",req.Path,req.Method);
                return false;
            }

            var requestData = new HttpRequestData();
            hasRes = await ParseRequestAsync(req, res,requestData, router);
            if (hasRes)
            {
                return hasRes;
            }

            //数据收集成功后，需要收集一些额外的数据
            hasRes = await ParsePostRequestAsync(req, res, router, requestData);
            if (hasRes)
            {
                return hasRes;
            }

            hasRes = await BeforeAsyncCall(req, res, requestData, router);
            if (hasRes)
            {
                return hasRes;
            }
            var parameters = router.InvokeMethod.GetParameters();
            if (parameters.Length !=1 && parameters.Length !=2)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                await res.WriteAsync("parameters length not match");
                this._logger.LogError("parameters length not match");
                return true;
            }

            var reqType = parameters[0].ParameterType;
            try
            {
                object invokeParam = ParseInvokeParameter(requestData,reqType);
                object retVal = router.InvokeMethod.Invoke(router.InvokeService,
                    parameters.Length == 1 ? new []{invokeParam} : new []{invokeParam,parameters[1].DefaultValue});

                if (retVal == null)
                {
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await res.WriteAsync("InternalServerError");
                    this._logger.LogError("InternalServerError:call rpc method and return null");
                    return true;
                }

                hasRes = await ProcessOutput(req, res,router, retVal);

            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                await res.WriteAsync($"InternalServerError:{ex.Message}");
                this._logger.LogError(ex, "call error:");
                hasRes = true;
            }

            return hasRes;

        }

        private async Task<bool> ParsePostRequestAsync(HttpRequest req, HttpResponse res, RouteItem router, HttpRequestData requestData)
        {
            bool result = false;
            IHttpPostParsePlugin plugin = null;
            if (router.Plugin != null && router.Plugin is IHttpPostParsePlugin)
            {
                plugin = router.Plugin as IHttpPostParsePlugin;
            }

            if (plugin != null)
            {
                result = await plugin.PostParseAsync(req, res, requestData, router);
                return result;
            }

            return result;
        }

        private async Task<bool> ProcessOutput(HttpRequest req, HttpResponse res,RouteItem routeItem, object retVal)
        {
            var resMsg = await GetJsonResult(retVal);
            SetContentType(req, res, resMsg);


            //输出前置处理
            if (routeItem.Plugin is IHttpPostProcessPlugin postPlugin)
            {
                var ret = await postPlugin.PostProcessAsync(req, res, resMsg, routeItem);
                if (ret)
                {
                    return ret;
                }
            }

            if (routeItem.Plugin is IHttpOutputPlugin outputPlugin)
            {
                var ret = await outputPlugin.OutputAsync(req, res, resMsg, routeItem);
                return ret;
            }
            //response
            await res.WriteAsync("{\"");
            await res.WriteAsync(this._gatewayOptions.WrapperCodeFieldName);
            await res.WriteAsync("\":");
            await res.WriteAsync(resMsg.Code.ToString());
            await res.WriteAsync(",\"");
            await res.WriteAsync(this._gatewayOptions.WrapperMessageFieldName);
            await res.WriteAsync("\":\"");
            await res.WriteAsync(resMsg.Message ?? "");
            await res.WriteAsync("\"");
            if (resMsg.Data != null)
            {
                string dataJson = this._jsonParser.ToJson(resMsg.Data);
                await res.WriteAsync(",\"");
                await res.WriteAsync(this._gatewayOptions.WrapperDataFieldName);
                await res.WriteAsync("\":");
                await res.WriteAsync(dataJson);
            }
            else
            {
                await res.WriteAsync(",\"");
                await res.WriteAsync(this._gatewayOptions.WrapperDataFieldName);
                await res.WriteAsync("\":{}");
            }
            await res.WriteAsync("}");
            return true;

        }

        private async Task<IJsonResult> GetJsonResult(object retVal)
        {
            var resMsg = CreateJsonResult();
            var retValType = retVal.GetType();
            if (retValType == typeof(Task))
            {
                return resMsg;
            }

            var tType = retValType.GenericTypeArguments[0];
            if (tType == typeof(RpcResult))
            {
                Task<RpcResult> retTask = retVal as Task<RpcResult>;
                var result = await retTask;
                resMsg.Code = result.Code;
                return resMsg;
            }


            if (tType.IsGenericType )
            {
                var retTask = retVal as Task;
                await retTask.AnyContext();



                var resultProp = retValType.GetProperty("Result");
                if (resultProp == null)
                {
                    resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    resMsg.Message = "type error";
                    return resMsg;
                }

                object result = resultProp.GetValue(retVal);


                var codeProp = tType.GetProperty("Code");
                if (codeProp == null)
                {
                    resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    resMsg.Message = "type error";
                    return resMsg;
                }
                resMsg.Code = (int)codeProp.GetValue(result);

                object dataVal = null;
                var dataProp = tType.GetProperty("Data");
                if (dataProp != null)
                {
                    dataVal = dataProp.GetValue(result);
                }
                resMsg.Data = dataVal;
                ExtractMessage(resMsg, dataVal);
                return resMsg;
            }

            resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            resMsg.Message = "INTERNAL_ERROR";
            return resMsg;
        }


        /// <summary>
        /// Extract Return Message
        /// </summary>
        /// <param name="result"></param>
        /// <param name="innerData"></param>
        protected virtual void ExtractMessage(IJsonResult result, object innerData)
        {
            if (innerData != null)
            {
                var p = innerData.GetType().GetProperty("ReturnMessage");
                if (p != null && p.PropertyType == typeof(string))
                {
                    var objValue = p.GetValue(innerData);
                    result.Message = objValue != null ? objValue.ToString() : "";
                }
            }
        }

        protected virtual IJsonResult CreateJsonResult()
        {
            return new JsonResult();
        }
        protected virtual void SetContentType(HttpRequest req, HttpResponse res, object retVal)
        {
            res.ContentType = "application/json;charset=utf-8";
        }

        protected virtual object ParseInvokeParameter(HttpRequestData requestData, Type reqType)
        {
            object req = null;
            if (!string.IsNullOrEmpty(requestData.RawBody))
            {
                req = this._jsonParser.FromJson(requestData.RawBody, reqType);
            }

            return ParseParameterFromDictionary(req,reqType, requestData.QueryOrFormData);
            /**
            var json = this._jsonParser.ToJson(requestData.QueryOrFormData);

            //this._logger.LogInformation("{1}==========================\r\n{0}",json,reqType.FullName);
            return this._jsonParser.FromJson(json, reqType);
            */
        }

        protected virtual object ParseParameterFromDictionary(object obj,Type reqType,IDictionary<string,string> dictData)
        {
             obj = obj??Activator.CreateInstance(reqType);

            var properties = reqType.GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanWrite )
                {
                    continue;
                }

                var name = property.Name.ToLower();
                //this._logger.LogInformation("{0}=-----------",name);
                if (dictData.ContainsKey(name))
                {
                    string value = dictData[name];
                    if(string.IsNullOrEmpty(value))
                    {
                        if (property.PropertyType == typeof(string))
                            property.SetValue(obj, "", null);

                        continue;
                    }

                    if (property.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(property.PropertyType, value);
                        property.SetValue(obj, enumValue, null);
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(value, property.PropertyType), null);
                    }

                }
            }

            return obj;
        }

        protected virtual async Task<bool> BeforeAsyncCall(HttpRequest req, HttpResponse res, HttpRequestData requestData, RouteItem routeItem)
        {
            if(_parsers != null)
            {
                foreach(var parser in this._parsers)
                {
                    var ret= await parser.ParseAsync(req, res, requestData, routeItem);
                    if (ret)
                    {
                        return ret;
                    }
                }
            }

            return false;
        }

        private async Task<bool> ParseRequestAsync(HttpRequest req, HttpResponse res,HttpRequestData requestData,RouteItem router)
        {
            bool result = false;
            IHttpParsePlugin plugin = null;
            if (router.Plugin is IHttpParsePlugin parsePlugin)
            {
                plugin = parsePlugin;
            }

            requestData.MessageId = router.MessageId;
            requestData.ServiceId = router.ServiceId;

            if (plugin != null)
            {
                result = await plugin.ParseAsync(req, res, requestData, router);
                CollectCommonData(req, requestData.QueryOrFormData);
                return result;
            }


            try
            {
                ProcessRequestData(req, router, requestData);
                CollectCommonData(req, requestData.QueryOrFormData);
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                await res.WriteAsync("InternalServerError:" + ex.Message);
                _logger.LogError(ex, "cast http request to rpc error");
                return true;
            }


            return result;
        }
        /// <summary>
        /// full equals match
        /// </summary>
        /// <param name="except">The except.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual bool Match(string except, string value)
        {
            return string.Equals(except, value, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual void ProcessRequestData(HttpRequest req, RouteItem router, HttpRequestData requestData)
        {
            var method = req.Method.ToLower();

            requestData.ServiceId = router.ServiceId;
            requestData.MessageId = router.MessageId;

            CollectQuery(req.Query, requestData.QueryOrFormData);
            string contentType = "";
            if (method == "post" || method == "put")
            {
                if (!string.IsNullOrEmpty(req.ContentType))
                {
                    contentType = req.ContentType.ToLower().Split(';')[0];
                }
            }

            if (contentType == "application/x-www-form-urlencoded" || contentType == "multipart/form-data")
                CollectForm(req.Form, requestData.QueryOrFormData);
            else if (contentType == "application/json")
                requestData.RawBody = CollectBody(req.Body);
        }

        /// <summary>
        /// collect common data;
        /// </summary>
        /// <param name="request"></param>
        /// <param name="collDataDict"></param>
        protected virtual void CollectCommonData(HttpRequest request, IDictionary<string, string> collDataDict)
        {
            //add request ip
            var ip = request.GetUserIp();
            if (collDataDict.ContainsKey(Constants.CLIENT_IP_FIELD_NAME))
            {
                collDataDict.Remove(Constants.CLIENT_IP_FIELD_NAME);
            }
            collDataDict.Add(Constants.CLIENT_IP_FIELD_NAME, ip);

            if (collDataDict.ContainsKey(Constants.IDENTITY_FIELD_NAME))
            {
                collDataDict.Remove(Constants.IDENTITY_FIELD_NAME);
            }

            //登录用户标识
            if (request.HttpContext.User.Identity.IsAuthenticated)
            {
                collDataDict.Add(Constants.IDENTITY_FIELD_NAME,
                    request.HttpContext.User.Identity.Name);

                //将所有的Claims 全局加到Dict中
                request.HttpContext.User.Claims.ForEach(item => collDataDict.Add(item.Type, item.Value));
            }
            else
            {
                collDataDict.Add(Constants.IDENTITY_FIELD_NAME, ""); //没有登录
            }

            //从Head中提取 x-request-id
            var requestId = string.Empty;
            if (request.Headers.ContainsKey(Constants.REQUEST_ID_HEAD_NAME))
            {
                bool hasSV = request.Headers.TryGetValue(Constants.REQUEST_ID_HEAD_NAME, out var sv);
                if (hasSV && sv.Count > 0)
                {
                    requestId = sv[0];
                }
            }
            if (string.IsNullOrEmpty(requestId))
            {
                requestId = Guid.NewGuid().ToString("N");
            }
            collDataDict.Add(Constants.REQUEST_ID_FIELD_NAME, requestId);
        }


        private RouteItem FindRouter(HttpRequest req)
        {
            string path = req.Path;
            RestfulVerb verb = CastVerb(req.Method);
            string cacheKey = string.Concat(path, ":", req.Method.ToLower());

            if (ROUTER_CACHE.ContainsKey(cacheKey))
            {
                return ROUTER_CACHE[cacheKey];
            }

            foreach (var router in this._routeOptions.Items)
            {
                // 没有配置Method标识匹配所有请求，否则必须匹配对应的Method
                if ( (verb & router.AcceptVerb) !=0)
                {
                    var match = Match(router.Path, path);

                    if (!match) continue;

                    ROUTER_CACHE.TryAdd(cacheKey, router);
                    return router;
                }
            }
            return null;
        }


        private RestfulVerb CastVerb(string reqMethod)
        {
            var method = reqMethod.ToLower();
            if (VerbsCache.ContainsKey(method))
            {
                return VerbsCache[method];
            }

            return RestfulVerb.UnKnown;
        }

        private string CollectBody(Stream body)
        {
            string bodyText;
            using (var reader = new StreamReader(body))
            {
                bodyText = reader.ReadToEnd();
            }
            return bodyText;
        }

        private void CollectForm(IFormCollection form, IDictionary<string, string> routeData)
        {
            foreach (var key in form.Keys)
            {
                var lowKey = key.ToLower();
                if (routeData.ContainsKey(lowKey))
                    routeData[lowKey] = form[lowKey];
                else
                    routeData.Add(lowKey, form[lowKey]);
            }
        }

        private void CollectQuery(IQueryCollection query, IDictionary<string, string> routeData)
        {
            foreach (var key in query.Keys)
            {
                var lowKey = key.ToLower();
                if (routeData.ContainsKey(lowKey))
                    routeData[lowKey] = query[lowKey];
                else
                    routeData.Add(lowKey, query[lowKey]);
            }
        }


    }
}
