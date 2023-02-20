// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace DotBPE.Rpc.Client.Impl
{
    public class DefaultDynamicInvokerFactory : IDynamicInvokerFactory
    {

        private readonly ConcurrentDictionary<int, Type> _serviceTypeCache = new ConcurrentDictionary<int, Type>();
        private readonly ConcurrentDictionary<string, IDynamicInvoker> _invokerCache = new ConcurrentDictionary<string, IDynamicInvoker>();
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly ILogger<DefaultDynamicInvokerFactory> _logger;

        private readonly MethodInfo _dynamicCreateGenericMethod;

        public DefaultDynamicInvokerFactory(IClientProxy clientProxy
            , IJsonParser jsonParser
            , ILogger<DefaultDynamicInvokerFactory> logger)
        {
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _logger = logger;

            _dynamicCreateGenericMethod = GetType().GetMethod("CreateDynamicInvokerCore", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Bind(Assembly[] assemblies)
        {
            if (assemblies.Any())
            {
                assemblies.ForEach(a =>
                {
                    BindAssemblyCore(a);
                });
            }
        }

        public IDynamicInvoker GetDynamicInvoker(int serviceId, ushort messageId)
        {
            var methodId = $"{serviceId}.{messageId}";

            if (_invokerCache.TryGetValue(methodId, out var cachedInvoker))
            {
                return cachedInvoker;
            }

            if (_serviceTypeCache.TryGetValue(serviceId, out var serviceType))
            {
                var invoker = CreateDynamicInvoker(serviceType, messageId);

                if (invoker != null)
                {
                    _invokerCache.TryAdd(methodId, invoker);
                    return invoker;
                }
            }

            var message = $"ServiceId={serviceId} is invalid, please check if it has been registered";
            _logger.LogError(message);
            throw new RpcException(message);
        }

        private void BindAssemblyCore(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsInterface)
                {
                    BindService(type);
                }
            }
        }

        private void BindService(Type serviceType)
        {
            var serviceAttr = serviceType.GetCustomAttribute<RpcServiceAttribute>();
            if (serviceAttr != null)
            {
                if (!_serviceTypeCache.TryAdd(serviceAttr.ServiceId, serviceType))
                {
                    if (_serviceTypeCache.TryGetValue(serviceAttr.ServiceId, out var cachedServiceType))
                    {
                        _logger.LogError($"Same service Id:{serviceAttr.ServiceId},{serviceType.FullName} and {cachedServiceType.FullName}");
                    }
                }

            }
        }


        private IDynamicInvoker CreateDynamicInvoker(Type serviceType, ushort messageId)
        {
            var methodHandler = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x =>
            {
                return x.GetCustomAttribute<RpcMethodAttribute>()?.MessageId == messageId;
            });

            if (methodHandler == null)
            {
                return null;
            }

            var returnType = methodHandler.ReturnType;
            var requestType = methodHandler.GetParameters()[0].ParameterType;

            if (!returnType.IsGenericType && returnType.GenericTypeArguments.Length != 1)
            {
                throw new RpcException($"Unexpected error, the return value  is not RPCResult<T>");
            }

            var returnGenericTypes = returnType.GenericTypeArguments[0];
            if (!returnGenericTypes.IsGenericType || returnGenericTypes.GetGenericTypeDefinition() != typeof(RpcResult<>))
            {
                throw new RpcException($"Unexpected error, the return value  is not RPCResult<T>");
            }

            var responseType = returnGenericTypes.GetGenericArguments()[0];


            var dynamicCreateInvoker = _dynamicCreateGenericMethod.MakeGenericMethod(serviceType, requestType, responseType);
            return dynamicCreateInvoker.Invoke(this, new object[] { methodHandler }) as IDynamicInvoker;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private DefaultDynamicInvoker<TService, TRequest, TResponse> CreateDynamicInvokerCore<TService, TRequest, TResponse>(MethodInfo methodHandler)
#pragma warning restore IDE0051 // Remove unused private members
           where TService : class
           where TRequest : class
           where TResponse : class
        {
            var parameters = methodHandler.GetParameters();

            if (parameters?.Length == 1) //without timeout
            {
                var serviceMethod = CreateServiceMethod<ServiceMethod<TService, TRequest, TResponse>, TRequest, TResponse>(methodHandler);
                var invoker = new MethodInvoker<TService, TRequest, TResponse>(serviceMethod, null, 0);

                return new DefaultDynamicInvoker<TService, TRequest, TResponse>(_clientProxy, _jsonParser, invoker);
            }
            else //with timeout
            {
                var serviceMethod = CreateServiceMethod<ServiceMethodWithTimeout<TService, TRequest, TResponse>, TRequest, TResponse>(methodHandler);
                var invoker = new MethodInvoker<TService, TRequest, TResponse>(null, serviceMethod, (int)parameters[1].DefaultValue);
                return new DefaultDynamicInvoker<TService, TRequest, TResponse>(_clientProxy, _jsonParser, invoker);
            }
        }

        private TDelegate CreateServiceMethod<TDelegate, TRequest, TResponse>(MethodInfo methodHandler)
             where TDelegate : Delegate
             where TRequest : class
             where TResponse : class
        {


            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), methodHandler);

        }
    }
}
