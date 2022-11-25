// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway.Internal
{
    internal class ApiProviderServiceBinder<TService>
        where TService : class
    {
        private readonly ApiMethodProviderContext<TService> _context;
        private readonly RpcGatewayOption _gatewayOption;
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly ILoggerFactory _loggerFactory;

        private readonly Type _serviceType;
        private readonly ILogger _logger;

        private readonly MethodInfo _dynamicCreateGenericMethod;
        private readonly MethodInfo _dynamicAddGenericMethod;

        public ApiProviderServiceBinder(ApiMethodProviderContext<TService> context
            , RpcGatewayOption gatewayOption
            , IClientProxy clientProxy
            , IJsonParser jsonParser
            , ILoggerFactory loggerFactory)
        {
            _context = context;
            _gatewayOption = gatewayOption;
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _loggerFactory = loggerFactory;

            _serviceType = typeof(TService);
            _logger = loggerFactory.CreateLogger<ApiProviderServiceBinder<TService>>();

            _dynamicCreateGenericMethod = GetType().GetMethod("CreateMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            _dynamicAddGenericMethod = GetType().GetMethod("AddMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal void Bind()
        {
            if (!_serviceType.IsInterface)
                return;

            //this._logger.LogInformation(type.FullName);
            var sAttr = _serviceType.GetCustomAttribute<RpcServiceAttribute>();
            if (sAttr == null)
                return;

            AddApiService(sAttr);
        }

        private void AddApiService(RpcServiceAttribute sAttr)
        {
            var methods = _serviceType.GetMethods();
            var serviceName = _serviceType.Name;
            foreach (var m in methods)
            {
                var mAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (mAttr == null)
                    continue;

                var rAttr = m.GetCustomAttribute<HttpRouteAttribute>();
                if (rAttr == null)
                    continue;

                var returnType = m.ReturnType;
                var requestType = m.GetParameters()[0].ParameterType;

                if (!returnType.IsGenericType && returnType.GenericTypeArguments.Length != 1)
                {
                    _logger.LogWarning("{serviceName}.{methodName} return type is not 'RpcResult<T>'", serviceName, m.Name);

                    continue;
                }

                var returnGenericTypes = returnType.GenericTypeArguments[0]; //RpcReslut<>
                if (!returnGenericTypes.IsGenericType || returnGenericTypes.GetGenericTypeDefinition() != typeof(RpcResult<>))
                {
                    _logger.LogWarning("{serviceName}.{methodName} return type is not 'RpcResult<T>'", serviceName, m.Name);
                    continue;
                }
                var responseType = returnGenericTypes.GetGenericArguments()[0];

                DynamicAddMethod(m, sAttr, mAttr, rAttr, requestType, responseType);

            }

        }

        private void DynamicAddMethod(MethodInfo m, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr, HttpRouteAttribute rAttr, Type requestType, Type responseType)
        {
            var dynamicCreateMethodInvoker = _dynamicCreateGenericMethod.MakeGenericMethod(requestType, responseType);
            var dynamicAddMethodInvoker = _dynamicAddGenericMethod.MakeGenericMethod(requestType, responseType);
            object method = dynamicCreateMethodInvoker.Invoke(this, new object[] { _serviceType.Name, m });
            dynamicAddMethodInvoker.Invoke(this, new object[] { method, sAttr, mAttr, rAttr });
        }


#pragma warning disable IDE0051 // Remove unused private members
        private ApiMethod<TRequest, TResponse> CreateMethod<TRequest, TResponse>(string serviceName, MethodInfo hanlder)
#pragma warning restore IDE0051 // Remove unused private members
            where TRequest : class
            where TResponse : class

        {
            return new ApiMethod<TRequest, TResponse>(serviceName, hanlder);
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void AddMethod<TRequest, TResponse>(ApiMethod<TRequest, TResponse> method
#pragma warning restore IDE0051 // Remove unused private members
            , RpcServiceAttribute sAttr
            , RpcMethodAttribute mAttr
            , HttpRouteAttribute rAttr)
           where TRequest : class
           where TResponse : class
        {


            if (rAttr.AcceptVerb == HttpVerb.Any)
            {
                var anyVerbs = new HttpVerb[] { HttpVerb.Get, HttpVerb.Post };

                foreach (var verb in anyVerbs)
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

        private void AddMethodCore<TRequest, TResponse>(ApiMethod<TRequest, TResponse> method, HttpApiOptions httpApiOptions)
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

                var routePattern = RoutePatternFactory.Parse(pattern);
                var parameters = method.HandlerMethod.GetParameters();

                if (parameters.Length == 1)
                {
                    var (invoker, metadata) = CreateModelCore<ServiceMethod<TService, TRequest, TResponse>, TRequest, TResponse>(method, httpApiOptions);

                    var methodInvoker = new ApiMethodInvoker<TService, TRequest, TResponse>(invoker, null, 0, _clientProxy);

                    var callHandler = new HttpApiCallHandler<TService, TRequest, TResponse>(_gatewayOption, methodInvoker, _jsonParser, httpApiOptions, _loggerFactory);

                    _context.AddMethod<TRequest, TResponse>(method, routePattern, metadata, callHandler.HandleCallAsync);
                }
                else //has timeout
                {

                    var (invokerWithTimeout, metadata) = CreateModelCore<ServiceMethodWithTimeout<TService, TRequest, TResponse>, TRequest, TResponse>(method, httpApiOptions);

                    var methodInvoker = new ApiMethodInvoker<TService, TRequest, TResponse>(null, invokerWithTimeout, (int)parameters[1].DefaultValue, _clientProxy);

                    var callHandler = new HttpApiCallHandler<TService, TRequest, TResponse>(_gatewayOption, methodInvoker, _jsonParser, httpApiOptions, _loggerFactory);

                    _context.AddMethod<TRequest, TResponse>(method, routePattern, metadata, callHandler.HandleCallAsync);
                }


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding {method.Name} on {typeof(TService).Name} to HTTP API.", ex);
            }
        }

        private (TDelegate invoker, List<object> metadata) CreateModelCore<TDelegate, TRequest, TResponse>(
           ApiMethod<TRequest, TResponse> method,
           HttpApiOptions httpApiOptions
         )
           where TDelegate : Delegate
           where TRequest : class
           where TResponse : class
        {

            var invoker = (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method.HandlerMethod);

            var metadata = new List<object>
            {

                new HttpMethodMetadata(new[] { httpApiOptions.AcceptVerb.ToString().ToUpper() }),

                // Add service method descriptor.
                // Is used by swagger generation to identify HTTP APIs.
                new HttpApiMetadata(method.HandlerMethod, httpApiOptions, typeof(TRequest), typeof(TResponse))
            };

            return (invoker, metadata);
        }
    }
}
