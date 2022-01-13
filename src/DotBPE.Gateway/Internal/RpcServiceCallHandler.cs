using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Internal
{
    internal class RpcServiceCallHandler<TService, TRequest, TResponse>
       where TService : class
       where TRequest : class
       where TResponse : class
    {
        private readonly RpcServiceMethodInvoker<TService, TRequest, TResponse> _serviceMethodInvoker;
     
        private readonly IJsonParser _jsonParser;

        private readonly ILogger _logger;

        private static readonly List<string> AllowContentTypes = new List<string>() {
            "application/x-www-form-urlencoded", "multipart/form-data","application/json" };

        private readonly HttpApiOptions _httpApiOption;

        public RpcServiceCallHandler(
            RpcServiceMethodInvoker<TService, TRequest, TResponse> serviceMethodInvoker
           ,IJsonParser jsonParser
           ,HttpApiOptions httpApiOption
           ,ILoggerFactory loggerFactory
           )
        {
            _serviceMethodInvoker = serviceMethodInvoker;
            _jsonParser = jsonParser;
          
            _httpApiOption = httpApiOption;
            _logger = loggerFactory.CreateLogger<RpcServiceCallHandler<TService, TRequest, TResponse>>();
        }

        public async Task HandleCallAsync(HttpContext httpContext)
        {
            var serviceProvider = httpContext.RequestServices;

            IHttpApiOutputProcess outputProcess = serviceProvider.GetService<IHttpApiOutputProcess>();
            IHttpApiErrorProcess errorProcess = serviceProvider.GetService<IHttpApiErrorProcess>();

            IHttpPlugin plugin = null;

            if (_httpApiOption.PluginType != null)
            {
                plugin = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, _httpApiOption.PluginType) as IHttpPlugin;
            }
            var selectedEncoding = ResponseEncoding.SelectCharacterEncoding(httpContext.Request);


            if (plugin != null && plugin is IHttpProcessPlugin processPlugin)
            {
                await processPlugin.ProcessAsync(httpContext.Request, httpContext.Response);
                return;
            }

            (object, StatusCode, string) tuple;
            if (plugin != null && plugin is IHttpRequestParsePlugin parsePlugin)
            {
                //replace request message parse
                tuple = await parsePlugin.ParseAsync(httpContext.Request);
            }
            else
            {
                try
                {
                    tuple = await CreateMessage(httpContext.Request);
                }
                catch (Exception ex)
                {
                    tuple = (null, StatusCode.InvalidArgument, ex.Message);
                }
               
            }


            var (requestMessage, requestStatusCode, errorMessage) = tuple;

            if (requestMessage == null || requestStatusCode != StatusCode.OK)
            {
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, errorMessage ?? string.Empty, requestStatusCode);
                return;
            }

            //post parse
            if (plugin != null && plugin is IHttpRequestParsePostPlugin parsePostPlugin)
            {
                (requestStatusCode, errorMessage) = await parsePostPlugin.ParseAsync(httpContext.Request, requestMessage);

                if (requestMessage == null || requestStatusCode != StatusCode.OK)
                {
                    await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, errorMessage ?? string.Empty, requestStatusCode);
                    return;
                }
            }

            RpcResult<TResponse> result;
            try
            {
                result = await _serviceMethodInvoker.Invoke(httpContext, (TRequest)requestMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, "Exception was thrown by handler.", StatusCode.Internal);
                return;
            }

            if (result == null || result.Code != 0)
            {               
                await SendErrorResponse(errorProcess, httpContext.Response, selectedEncoding, "Unknown Error", StatusCode.Unknown );
                return;
            }

            await SendResponse(outputProcess, plugin, httpContext.Response, selectedEncoding, result);
        }

     

        private async Task<(object requestMessage, StatusCode statusCode, string errorMessage)> CreateMessage(HttpRequest request)
        {
          
            var method = request.Method.ToLower();
            var contentType = "";

            if (method == "post" || method == "put" || method == "patch" || method == "delete")
            {
                if (!string.IsNullOrEmpty(request.ContentType))
                {
                    contentType = request.ContentType.ToLower().Split(';')[0];
                }

                if (!AllowContentTypes.Contains(contentType))
                {
                    return (null, StatusCode.InvalidArgument, "Request content-type is invalid.");
                }
            }

            Dictionary<string,string> requestValues = new Dictionary<string, string>();
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
                        return (null, StatusCode.InvalidArgument, exception.Message);
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

            return (requestMessage, StatusCode.OK, null);
        }

        private void ParseFromDictionary(TRequest instance, IDictionary<string, string> requestValues)
        {          

            var properties =  typeof(TRequest).GetProperties();

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
        private void ParseQueryStringValues(IQueryCollection query, Dictionary<string, string> requestValues)
        {
            foreach (var key in query.Keys)
            {
                var lowKey = ToFriendlyKey(key);
                if (!requestValues.ContainsKey(lowKey))
                    requestValues.Add(lowKey, query[key]);
            }
        }

        private void ParseRouteValues(RouteValueDictionary routeValues, Dictionary<string, string> requestValues)
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

        private void ParseFormValues(IFormCollection form, Dictionary<string, string> requestValues)
        {

            foreach (var key in form.Keys)
            {
                var lowKey = ToFriendlyKey(key);
                if (!requestValues.ContainsKey(lowKey))                  
                    requestValues.Add(lowKey, form[key]);
            }
        }

        private async Task SendResponse(IHttpApiOutputProcess outputProcess, IHttpPlugin plugin, HttpResponse response, Encoding encoding, RpcResult<TResponse> message)
        {
            object responseBody = message;
            var processed = false;
            if (plugin != null && plugin is IHttpOutputProcessPlugin outputProcessPlugin)
            {
                processed = await outputProcessPlugin.ProcessAsync(response, encoding, responseBody);
            }
            if (processed)
            {
                return;
            }

            if (outputProcess != null)
            {
                (processed, responseBody) = await outputProcess.ProcessAsync(response, encoding, responseBody);
            }
            if (processed)
            {
                return;
            }
            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/json";

            await WriteResponseMessage(response, encoding, responseBody);
        }

        private async Task SendErrorResponse(IHttpApiErrorProcess errorProcess, HttpResponse response, Encoding encoding, string errorMessage,StatusCode statusCode)
        {
          
            var e = new Error() { ErrorCode = statusCode ,Message = errorMessage};

            var processed = false;

            if (errorProcess != null)
            {
                processed = await errorProcess.ProcessAsync(response, encoding, e);
            }
            if (processed)
            {
                return;
            }

            response.StatusCode = MapStatusCodeToHttpStatus(e.ErrorCode);
            response.ContentType = "application/json";

            await WriteResponseMessage(response, encoding, e);
        }

        private async Task WriteResponseMessage(HttpResponse response, Encoding encoding, object responseBody)
        {
            using (var writer = new HttpResponseStreamWriter(response.Body, encoding))
            {
                await writer.WriteAsync(_jsonParser.ToJson(responseBody));              

                // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
                // buffers. This is better than just letting dispose handle it (which would result in a synchronous
                // write).
                await writer.FlushAsync();
            }
        }

        private static int MapStatusCodeToHttpStatus(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.OK:
                    return StatusCodes.Status200OK;

                case StatusCode.Cancelled:
                    return StatusCodes.Status408RequestTimeout;

                case StatusCode.Unknown:
                    return StatusCodes.Status500InternalServerError;

                case StatusCode.InvalidArgument:
                    return StatusCodes.Status400BadRequest;

                case StatusCode.DeadlineExceeded:
                    return StatusCodes.Status504GatewayTimeout;

                case StatusCode.NotFound:
                    return StatusCodes.Status404NotFound;

                case StatusCode.AlreadyExists:
                    return StatusCodes.Status409Conflict;

                case StatusCode.PermissionDenied:
                    return StatusCodes.Status403Forbidden;

                case StatusCode.Unauthenticated:
                    return StatusCodes.Status401Unauthorized;

                case StatusCode.ResourceExhausted:
                    return StatusCodes.Status429TooManyRequests;

                case StatusCode.FailedPrecondition:
                    // Note, this deliberately doesn't translate to the similarly named '412 Precondition Failed' HTTP response status.
                    return StatusCodes.Status400BadRequest;

                case StatusCode.Aborted:
                    return StatusCodes.Status409Conflict;

                case StatusCode.OutOfRange:
                    return StatusCodes.Status400BadRequest;

                case StatusCode.Unimplemented:
                    return StatusCodes.Status501NotImplemented;

                case StatusCode.Internal:
                    return StatusCodes.Status500InternalServerError;

                case StatusCode.Unavailable:
                    return StatusCodes.Status503ServiceUnavailable;

                case StatusCode.DataLoss:
                    return StatusCodes.Status500InternalServerError;
            }

            return StatusCodes.Status500InternalServerError;
        }
        private static string ToFriendlyKey(string key)
        {
            var parts = key.ToLower().Split('_');

            return string.Join("", parts);
        }
    }
}
