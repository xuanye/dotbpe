using DotBPE.Gateway.Internal;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway
{
    internal class HttpApiProviderServiceBinder<TService> where TService : class
    {
        private readonly RpcGatewayOption _gatewayOption;
        private readonly RpcServiceMethodProviderContext<TService> _context;
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private readonly Type _serviceType;

        private readonly MethodInfo _dynamicCreateGenericMethod;
        private readonly MethodInfo _dynamicAddGenericMethod;
        public HttpApiProviderServiceBinder(           
            RpcServiceMethodProviderContext<TService> context,
            RpcGatewayOption gatewayOption,
            IClientProxy clientProxy,
            IJsonParser jsonParser,
            ILoggerFactory loggerFactory
         )
        {
            _serviceType = typeof(TService);
            _gatewayOption = gatewayOption;
            _context = context;
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<HttpApiProviderServiceBinder<TService>>();
            _dynamicCreateGenericMethod = this.GetType().GetMethod("CreateMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            _dynamicAddGenericMethod = this.GetType().GetMethod("AddMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void BindAll()
        {
            if (!_serviceType.IsInterface)
                 return;

            //this._logger.LogInformation(type.FullName);
            var sAttr = _serviceType.GetCustomAttribute<RpcServiceAttribute>();
            if (sAttr == null)
                return;

            AddRpcService( sAttr, new string[] {});
        }

        private void AddRpcService(RpcServiceAttribute sAttr, params string[] categories)
        {
            var methods = _serviceType.GetMethods();
            foreach (var m in methods)
            {
                var mAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (mAttr == null)
                    continue;

                var rAttr = m.GetCustomAttribute<RouterAttribute>();
                if (rAttr == null)
                    continue;

                Type returnType = m.ReturnType;
                Type requestType = m.GetParameters()[0].ParameterType;

                if (!returnType.IsGenericType && returnType.GenericTypeArguments.Length !=1)
                {
                    //TODO:WARNING~
                    continue;
                }
               
                var returnGenericTypes = returnType.GenericTypeArguments[0]; //RpcReslut<>
                if(!returnGenericTypes.IsGenericType || returnGenericTypes.GetGenericTypeDefinition() != typeof(RpcResult<>))
                {
                    //TODO:WARNING~
                    continue;
                }
                var responseType = returnGenericTypes.GetGenericArguments()[0];
                
                if ((categories != null && categories.Any() && categories.Contains(rAttr.Category))
                    || "default".Equals(rAttr.Category, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    DynamicAddMethod(m, sAttr, mAttr, rAttr, requestType, responseType);
                }
              
            }
        }
        private void DynamicAddMethod(MethodInfo m, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr, RouterAttribute rAttr, Type requestType, Type responseType)
        {
            var dynamicCreateMethodInvoker = _dynamicCreateGenericMethod.MakeGenericMethod(requestType, responseType);
            var dynamicAddMethodInvoker = _dynamicAddGenericMethod.MakeGenericMethod(requestType, responseType);
            object method = dynamicCreateMethodInvoker.Invoke(this, new object[] { _serviceType.Name, m });
            dynamicAddMethodInvoker.Invoke(this, new object[] { method, sAttr, mAttr, rAttr });
        }


        private Method<TRequest,TResponse> CreateMethod<TRequest, TResponse>(string serviceName,MethodInfo hanlder)
            where TRequest : class
            where TResponse : class

        {
            return new Method<TRequest, TResponse>(serviceName, hanlder);
        }

        private void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr, RouterAttribute rAttr)
             where TRequest : class
           where TResponse : class
        {
          

            if(rAttr.AcceptVerb== RestfulVerb.Any)
            {
                var anyVerbs = new RestfulVerb[] { RestfulVerb.Get, RestfulVerb.Post };

                foreach(var verb in anyVerbs)
                {
                    var httpApiOptions = new HttpApiOptions()
                    {
                        AcceptVerb = verb,
                        Category = rAttr.Category,
                        Pattern = rAttr.Path,
                        PluginName = rAttr.PluginName,
                        Version = rAttr.Version
                    };
                    AddMethodCore(method, httpApiOptions);
                }
             
               
            }
            else
            {
                var httpApiOptions = new HttpApiOptions()
                {
                    AcceptVerb = rAttr.AcceptVerb,
                    Category = rAttr.Category,
                    Pattern = rAttr.Path,
                    PluginName = rAttr.PluginName,
                    Version = rAttr.Version
                };
                AddMethodCore(method, httpApiOptions);
            }           
        }
        private void AddMethodCore<TRequest, TResponse>(Method<TRequest, TResponse> method, HttpApiOptions httpApiOptions)
           where TRequest : class
           where TResponse : class
        {
            try
            {
                var pattern = httpApiOptions.Pattern;

                if (!pattern.StartsWith('/'))
                {
                    // This validation is consistent with rpc-gateway code generation.
                    // We should match their validation to be a good member of the eco-system.
                    throw new InvalidOperationException($"Path template must start with /: {pattern}");
                }


                var methodContext = MethodOptions.Create();
                var routePattern = RoutePatternFactory.Parse(pattern);

                var parameters = method.HandlerMethod.GetParameters();
                if (parameters.Length == 1)
                {
                    var (invoker, metadata) = CreateModelCore<RpcServiceMethod<TService, TRequest, TResponse>, TRequest, TResponse>(method,httpApiOptions);

                    var methodInvoker = new RpcServiceMethodInvoker<TService, TRequest, TResponse>(invoker,null,0, method, methodContext, _clientProxy);

                    var unaryServerCallHandler = new RpcServiceCallHandler<TService, TRequest, TResponse>(_gatewayOption, methodInvoker, _jsonParser, httpApiOptions, _loggerFactory);

                    _context.AddMethod<TRequest, TResponse>(method, routePattern, metadata, unaryServerCallHandler.HandleCallAsync);
                }
                else //has timeout 
                {
                 
                    var (invokerWithTimeout, metadata) = CreateModelCore<RpcServiceMethodWithTimeout<TService, TRequest, TResponse>, TRequest, TResponse>(method, httpApiOptions);

                    var methodInvoker = new RpcServiceMethodInvoker<TService, TRequest, TResponse>(null, invokerWithTimeout, (int)parameters[1].DefaultValue, method, methodContext, _clientProxy);

                    var unaryServerCallHandler = new RpcServiceCallHandler<TService, TRequest, TResponse>(_gatewayOption, methodInvoker, _jsonParser, httpApiOptions, _loggerFactory);

                    _context.AddMethod<TRequest, TResponse>(method, routePattern, metadata, unaryServerCallHandler.HandleCallAsync);
                }

            
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding {method.Name} on {typeof(TService).Name} to HTTP API.", ex);
            }
        }

        private (TDelegate invoker, List<object> metadata) CreateModelCore<TDelegate, TRequest, TResponse>(
           Method<TRequest, TResponse> method,
           HttpApiOptions httpApiOptions
         )
           where TDelegate : Delegate
           where TRequest : class
           where TResponse : class
        {
          
            var invoker = (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method.HandlerMethod);

            var metadata = new List<object>();
            // Add type metadata first so it has a lower priority
            //metadata.AddRange(typeof(TService).GetCustomAttributes(inherit: true));
            // Add method metadata last so it has a higher priority
            //metadata.AddRange(handlerMethod.GetCustomAttributes(inherit: true));
            metadata.Add(new HttpMethodMetadata(new[] { httpApiOptions.AcceptVerb.ToString().ToUpper() }));

            // Add service method descriptor.
            // Is used by swagger generation to identify HTTP APIs.
            metadata.Add(new RpcHttpMetadata(method.HandlerMethod, httpApiOptions,typeof(TRequest),typeof(TResponse)));

            return (invoker, metadata);
        }
    }
}
