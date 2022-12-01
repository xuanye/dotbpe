// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DotBPE.Gateway.Internal
{

    internal class HttpApiCallHandler<TService, TRequest, TResponse>
      where TService : class
      where TRequest : class
      where TResponse : class
    {
        private const string _clientIpPropertyName = "ClientIp";
        private const string _identityPropertyName = "Identity";
        private const string _requestIdPropertyName = "XRequestId";

        private readonly RpcGatewayOption _gatewayOption;
        private readonly ApiMethodInvoker<TService, TRequest, TResponse> _methodInvoker;

        private readonly IJsonParser _jsonParser;

        private readonly ILogger _logger;

        private static readonly List<string> _allowContentTypes = new List<string>() {
            "application/x-www-form-urlencoded", "multipart/form-data","application/json" };

        private readonly HttpApiOptions _httpApiOption;

        public HttpApiCallHandler(
            RpcGatewayOption gatewayOption
           , ApiMethodInvoker<TService, TRequest, TResponse> methodInvoker
           , IJsonParser jsonParser
           , HttpApiOptions httpApiOption
           , ILoggerFactory loggerFactory
           )
        {
            _gatewayOption = gatewayOption;
            _methodInvoker = methodInvoker;
            _jsonParser = jsonParser;

            _httpApiOption = httpApiOption;
            _logger = loggerFactory.CreateLogger<HttpApiCallHandler<TService, TRequest, TResponse>>();
        }

        public async Task HandleCallAsync(HttpContext httpContext)
        {
            var serviceProvider = httpContext.RequestServices;

            var outputProcess = serviceProvider.GetService<IHttpApiOutputProcess>();
            var errorProcess = serviceProvider.GetService<IHttpApiErrorProcess>();

            IHttpPlugin plugin = null;

            if (_httpApiOption.PluginType != null)
            {
                plugin = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, _httpApiOption.PluginType) as IHttpPlugin;
            }
            var selectedEncoding = ResponseEncoding.SelectCharacterEncoding(httpContext.Request);


            RpcResult<TResponse> result;
            try
            {
                if (plugin != null && plugin is IHttpProcessPlugin processPlugin)
                {
                    var processedResult = await processPlugin.ProcessAsync(httpContext.Request, httpContext.Response);
                    if (processedResult != null)
                    {
                        if (processedResult is TResponse resMsg)
                        {
                            result = new RpcResult<TResponse>() { Data = resMsg };
                            await SendResponse(outputProcess, plugin, httpContext, selectedEncoding, result);
                        }
                    }
                    return;
                }

                (object, int, string) tuple;
                if (plugin != null && plugin is IHttpRequestParsePlugin parsePlugin)
                {
                    //replace request message parse
                    tuple = await parsePlugin.ParseAsync(httpContext.Request);
                }
                else
                {
                    tuple = await CreateMessage(httpContext.Request);
                }


                var (requestMessage, requestStatusCode, errorMessage) = tuple;

                if (requestMessage == null || (requestStatusCode != StatusCodes.Status200OK && requestStatusCode != 0))
                {
                    await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, errorMessage ?? string.Empty, requestStatusCode);
                    return;
                }

                //post parse
                if (plugin != null && plugin is IHttpRequestParsePostPlugin parsePostPlugin)
                {
                    (requestStatusCode, errorMessage) = await parsePostPlugin.ParseAsync(httpContext.Request, requestMessage);

                    if (requestMessage == null || (requestStatusCode != StatusCodes.Status200OK && requestStatusCode != 0))
                    {
                        await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, errorMessage ?? string.Empty, requestStatusCode);
                        return;
                    }
                }

                //ParseCommonParams
                ParseCommonParams(httpContext.Request, requestMessage);

                //invoke method
                result = await _methodInvoker.Invoke((TRequest)requestMessage);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, rpcEx.Message);
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, rpcEx.Message, rpcEx.StatusCode);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, ex.Message, StatusCodes.Status500InternalServerError);
                return;
            }

            if (result == null)
            {
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, "Unknown Error", StatusCodes.Status500InternalServerError);
                return;
            }

            await SendResponse(outputProcess, plugin, httpContext, selectedEncoding, result);
        }


        private void ParseCommonParams(HttpRequest request, object requestMsg)
        {

            var clientIp = request.GetClientIp();
            SetValue(requestMsg, _clientIpPropertyName, clientIp);
            string xRequestId;
            if (request.Headers.TryGetValue("X-Request-Id", out var requestId))
            {
                xRequestId = requestId.ToString();
            }
            else
            {
                xRequestId = Guid.NewGuid().ToString("N");
            }
            SetValue(requestMsg, _identityPropertyName, xRequestId);
            if (request.HttpContext.User.Identity.IsAuthenticated)
            {
                SetValue(requestMsg, _identityPropertyName, request.HttpContext.User.Identity.Name);
            }
            else
            {
                SetValue(requestMsg, _identityPropertyName, "");
            }

        }

        private static void SetValue(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName);
            property?.SetValue(obj, value);
        }

        private async Task<(object requestMessage, int statusCode, string errorMessage)> CreateMessage(HttpRequest request)
        {

            var method = request.Method.ToLower();
            var contentType = "";

            if (method == "post" || method == "put" || method == "patch" || method == "delete")
            {
                if (!string.IsNullOrEmpty(request.ContentType))
                {
                    contentType = request.ContentType.ToLower().Split(';')[0];
                }

                if (!_allowContentTypes.Contains(contentType))
                {
                    return (null, StatusCodes.Status400BadRequest, "Request content-type is invalid.");
                }
            }

            var requestValues = new Dictionary<string, string>();
            TRequest requestMessage;

            if (contentType == "application/x-www-form-urlencoded" || contentType == "multipart/form-data")
            {
                requestMessage = Activator.CreateInstance<TRequest>();

                var form = await request.ReadFormAsync();
                ParseFormValues(form, requestValues);
            }
            else if (contentType.StartsWith("application/json"))
            {

                var encoding = RequestEncoding.SelectCharacterEncoding(request);
                // TODO: Handle unsupported encoding

                using (var requestReader = new HttpRequestStreamReader(request.Body, encoding))
                {
                    try
                    {
                        var body = await requestReader.ReadToEndAsync();
                        requestMessage = _jsonParser.FromJson<TRequest>(body);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                        return (null, StatusCodes.Status400BadRequest, "Request JSON payload is not correctly formatted.");
                    }
                }
            }
            else
            {
                requestMessage = (TRequest)Activator.CreateInstance<TRequest>();
            }

            ParseRouteValues(request.RouteValues, requestValues);
            ParseQueryStringValues(request.Query, requestValues);
            ParseFromDictionary(requestMessage, requestValues);

            return (requestMessage, StatusCodes.Status200OK, null);
        }

        private static void ParseFromDictionary(TRequest instance, IDictionary<string, string> requestValues)
        {

            var properties = typeof(TRequest).GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                var name = property.Name.ToLower();
                //this._logger.LogInformation("{0}=-----------",name);
                if (requestValues.ContainsKey(name))
                {
                    string value = requestValues[name];
                    if (string.IsNullOrEmpty(value))
                    {
                        if (property.PropertyType == typeof(string))
                            property.SetValue(instance, "", null);

                        continue;
                    }

                    if (property.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(property.PropertyType, value);
                        property.SetValue(instance, enumValue, null);
                    }
                    else
                    {
                        property.SetValue(instance, Convert.ChangeType(value, property.PropertyType), null);
                    }

                }
            }
        }
        private static void ParseQueryStringValues(IQueryCollection query, Dictionary<string, string> requestValues)
        {
            foreach (var key in query.Keys)
            {
                var lowKey = ToFriendlyKey(key);
                if (!requestValues.ContainsKey(lowKey))
                    requestValues.Add(lowKey, query[key]);
            }
        }

        private static void ParseRouteValues(RouteValueDictionary routeValues, Dictionary<string, string> requestValues)
        {
            foreach (var key in routeValues.Keys)
            {
                var lowKey = ToFriendlyKey(key);
                if (!requestValues.ContainsKey(lowKey))
                {
                    requestValues.Add(lowKey, routeValues[key].ToString());
                }
                else
                {
                    requestValues[lowKey] = routeValues[key].ToString();
                }
            }
        }

        private static void ParseFormValues(IFormCollection form, Dictionary<string, string> requestValues)
        {

            foreach (var key in form.Keys)
            {
                var lowKey = ToFriendlyKey(key);
                if (!requestValues.ContainsKey(lowKey))
                    requestValues.Add(lowKey, form[key]);
            }
        }

        private async Task SendResponse(IHttpApiOutputProcess outputProcess, IHttpPlugin plugin, HttpContext context, Encoding encoding, RpcResult<TResponse> result)
        {

            var processed = false;
            if (plugin != null && plugin is IHttpOutputProcessPlugin outputProcessPlugin)
            {
                processed = await outputProcessPlugin.ProcessAsync(context.Request, context.Response, encoding, result);
            }
            if (processed)
            {
                return;
            }

            if (outputProcess != null)
            {
                processed = await outputProcess.ProcessAsync(context.Request, context.Response, encoding, result);
            }
            if (processed)
            {
                return;
            }
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            await WriteResponseMessage(context.Response, encoding, result);
        }

        private async Task SendErrorResponse(IHttpApiErrorProcess errorProcess, HttpResponse response, Encoding encoding, string errorMessage, int statusCode)
        {

            var e = new Error() { Code = statusCode, Message = errorMessage };

            var processed = false;

            if (errorProcess != null)
            {
                processed = await errorProcess.ProcessAsync(response, encoding, e);
            }
            if (processed)
            {
                return;
            }

            response.StatusCode = statusCode;
            response.ContentType = "application/json";

            await WriteResponseMessage(response, encoding, new RpcResult<Error>() { Code = statusCode, Data = e });
        }


        private async Task WriteResponseMessage<T>(HttpResponse response, Encoding encoding, RpcResult<T> result) where T : class
        {

            using var writer = new HttpResponseStreamWriter(response.Body, encoding);

            await writer.WriteAsync("{\"");
            await writer.WriteAsync(_gatewayOption.CodeFieldName);
            await writer.WriteAsync("\":");
            await writer.WriteAsync(result.Code.ToString());
            await writer.WriteAsync(",\"");
            await writer.WriteAsync(_gatewayOption.MessageFieldName);
            await writer.WriteAsync("\":\"");

            var message = ExtractMessage(result.Data);
            if (!string.IsNullOrEmpty(message))
            {
                await writer.WriteAsync(HttpUtility.JavaScriptStringEncode(message));
            }
            else
            {
                await writer.WriteAsync("");
            }
            await writer.WriteAsync("\"");

            if (result.Data != null)
            {
                await writer.WriteAsync(",\"");
                await writer.WriteAsync(_gatewayOption.DataFieldName);
                await writer.WriteAsync("\":");

                string jsonData = _jsonParser.ToJson(result.Data);
                if (string.IsNullOrEmpty(jsonData))
                {
                    await writer.WriteAsync("{}");
                }
                else
                {
                    await writer.WriteAsync(jsonData);
                }
            }
            else
            {
                await writer.WriteAsync(",\"");
                await writer.WriteAsync(_gatewayOption.DataFieldName);
                await writer.WriteAsync("\":{}");
            }
            await writer.WriteAsync("}");

            // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
            // buffers. This is better than just letting dispose handle it (which would result in a synchronous
            // write).
            await writer.FlushAsync();
        }
        private static string ExtractMessage(object data)
        {
            if (data != null)
            {
                var p = data.GetType().GetProperty("ReturnMessage");

                if (p == null)
                {
                    p = data.GetType().GetProperty("Message");
                }
                if (p != null && p.PropertyType == typeof(string))
                {
                    var objValue = p.GetValue(data);
                    return objValue != null ? objValue.ToString() : "";
                }
            }
            return null;
        }


        private static string ToFriendlyKey(string key)
        {
            var parts = key.ToLower().Split('_');

            return string.Join("", parts);
        }
    }
}
