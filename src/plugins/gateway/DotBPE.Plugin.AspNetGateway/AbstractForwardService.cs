using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Environment = DotBPE.Rpc.Environment;

namespace DotBPE.Plugin.AspNetGateway
{
    public abstract class AbstractForwardService<TMessage> : IForwardService where TMessage : InvokeMessage
    {
        private static ILogger Logger = Environment.Logger.ForType<AbstractForwardService<TMessage>>();

        private readonly HttpRouterOption _option;
        private readonly IRpcClient<TMessage> _client;

        private readonly CallInvoker<TMessage> _invoker;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rpcClient"></param>
        public AbstractForwardService(IRpcClient<TMessage> rpcClient, IOptionsSnapshot<HttpRouterOption> optionsAccessor)
        {
            Rpc.Utils.Preconditions.CheckNotNull(optionsAccessor.Value, "WebApiRouterOption");

            _option = optionsAccessor.Value;

            _client = rpcClient;

            _invoker = this.GetProtocolCallInvoker(rpcClient);
        }

        /// <summary>
        /// 讲Http请求转成RPC调用，发送到服务端，并接受其响应，并回复给Http请求调用方
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async Task<RpcContentResult> ForwardAysnc(HttpContext context)
        {
            //TODO:添加日志服务

            RpcContentResult result = new RpcContentResult();
            //1. 根据HttpContext 获取路由信息，
            RequestData rd = null;
            try
            {
                rd = ProcessRequestData(context);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = "Error Request" + ex.Message;

                return result;
            }
            //2. 根据路由配置 获取到 相应的ServiceId和MessageId
            //3. 如果ServiceId和MessageId 不存在则直接返回404 错误
            if (rd == null)
            {
                result.Code = 404;
                result.Message = "service not found!";

                return result;
            }

            //4. 如果存在则 从路由 Form, QueryString, Body 中获取请求数据, 转换成TMessage，通过rpcClient转发
            TMessage message = this.EncodeRequest(rd);

            try
            {
                TMessage rsp = await _invoker.AsyncCall(message, 3000);

                if (rsp != null)
                {
                    result.Data = this.MessageToJson(rsp);
                }
            }
            catch (RpcCommunicationException rpcEx)
            {
                result.Code = 500;
                result.Message = "timeout error:" + rpcEx.Message;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = "call error:" + ex.Message;
            }

            return result;
        }

        protected virtual void PreResponse(HttpContext context, TMessage resMessage)
        {
            var sessionId = "";
            var hasValue = context.Request.Cookies.TryGetValue(Constants.DOTBPE_SEESIONID, out sessionId);
            if (!string.IsNullOrEmpty(sessionId))
            {
                return; // 存在sessionId
            }

            if (this._option.CookieMode == CookieMode.Auto) // 自动添加sessionId
            {
                sessionId = Guid.NewGuid().ToString("N");
            }
            else if (this._option.CookieMode == CookieMode.Manual)
            {
                sessionId = GetSessionIdFromMessage(resMessage);
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                CookieOptions option = new CookieOptions();
                option.HttpOnly = true;
                context.Response.Cookies.Append(Constants.DOTBPE_SEESIONID, sessionId, option);
            }
        }

        protected virtual string GetSessionIdFromMessage(TMessage message)
        {
            return "";
        }

        /// <summary>
        /// 初始化对应协议的CallInvoker
        /// </summary>
        /// <param name="rpcClient">RPC链接</param>
        /// <returns></returns>
        protected abstract CallInvoker<TMessage> GetProtocolCallInvoker(IRpcClient<TMessage> rpcClient);

        /// <summary>
        /// 将请求信息转换成RPC请求的消息
        /// </summary>
        /// <param name="reqData">请求数据</param>
        /// <returns></returns>
        protected abstract TMessage EncodeRequest(RequestData reqData);

        /// <summary>
        /// 将Response Message 序列化成Json字符串
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected abstract string MessageToJson(TMessage message);

        /// <summary>
        /// 从Context中自定义提取数据到请求字典中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collDataDict"></param>
        protected virtual void AddFromContext(HttpContext context, Dictionary<string, string> collDataDict)
        {
            //从cookie中提取 dotbpe-session-id
            if (this._option.CookieMode != CookieMode.None)
            {
                var sessionId = "";
                var hasValue = context.Request.Cookies.TryGetValue(Constants.DOTBPE_SEESIONID, out sessionId);
                if (hasValue && !collDataDict.ContainsKey(Constants.DOTBPE_SEESIONID))
                {
                    collDataDict.Add(Constants.SEESIONID_FIELD_NAME, sessionId);
                }
            }
            //从Head中提取 x-request-id
            var requestId = string.Empty;
            if (context.Request.Headers.ContainsKey(Constants.X_REQUEST_ID))
            {
                Microsoft.Extensions.Primitives.StringValues sv;
                bool hasSV = context.Request.Headers.TryGetValue(Constants.X_REQUEST_ID, out sv);
                if (hasSV && sv.Count > 0)
                {
                    requestId = sv[0];
                }
            }
            if (string.IsNullOrEmpty(requestId))
            {
                requestId = Guid.NewGuid().ToString("D");
            }
            collDataDict.Add(Constants.REQUESTID_FIELD_NAME, requestId);
        }

        /// <summary>
        /// 从请求中提取请求数据到RequestData中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual RequestData ProcessRequestData(HttpContext context)
        {
            string path = context.Request.Path;
            string method = context.Request.Method.ToLower();
            for (var i = 0; i < _option.Items.Count; i++)
            {
                var router = _option.Items[i];
                // 没有配置Method标识匹配所有请求，否则必须匹配对应的Method
                if (string.IsNullOrEmpty(router.Method) || router.Method.Equals("all", StringComparison.OrdinalIgnoreCase)
                    || router.Method.Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Match(router.Path, path);
                    if (match)
                    {
                        RequestData rd = new RequestData();
                        rd.ServiceId = router.ServiceId;
                        rd.MessageId = router.MessageId;

                        rd.Data = new Dictionary<string, string>();

                        CollectQuery(context.Request.Query, rd.Data);
                        string contentType = "";
                        if (method == "post" || method == "put")
                        {
                            contentType = context.Request.ContentType.ToLower().Split(';')[0];
                        }

                        if (contentType == "application/x-www-form-urlencoded"
                             || contentType == "multipart/form-data"
                             )
                        {
                            CollectForm(context.Request.Form, rd.Data);
                        }

                        if (contentType == "application/json")
                        {
                            rd.RawBody = CollectBody(context.Request.Body);
                        }

                        if (context.User.Identity.IsAuthenticated)
                        {
                            //添加当前用户; 实际的项目中可根据自己的情况去扩展
                            rd.Data.Add(Constants.IDENTITY_FIELD_NAME, context.User.Identity.Name);
                        }
                        //添加客户端IP 项目中可根据实际情况添加需要的内容
                        var IPAddress = context.Connection.RemoteIpAddress;
                        string ip = IPAddress.IsIPv4MappedToIPv6 ? IPAddress.MapToIPv4().ToString() : IPAddress.ToString();
                        rd.Data.Add(Constants.CLIENTIP_FIELD_NAME, ip);

                        //自定义数据
                        AddFromContext(context, rd.Data);

                        return rd;
                    }
                }
            }

            return null;
        }

        protected virtual bool Match(string except, string value)
        {
            return string.Equals(except, value, StringComparison.OrdinalIgnoreCase);
        }

        private string CollectBody(Stream body)
        {
            string bodyText = null;
            using (StreamReader reader = new StreamReader(body))
            {
                bodyText = reader.ReadToEnd();
            }
            return bodyText;
        }

        private void CollectForm(IFormCollection form, Dictionary<string, string> routeData)
        {
            foreach (string key in form.Keys)
            {
                if (routeData.ContainsKey(key))
                    routeData[key] = form[key];
                else
                    routeData.Add(key, form[key]);
            }
        }

        private void CollectQuery(IQueryCollection query, Dictionary<string, string> routeData)
        {
            foreach (string key in query.Keys)
            {
                if (routeData.ContainsKey(key))
                    routeData[key] = query[key];
                else
                    routeData.Add(key, query[key]);
            }
        }
    }
}
