// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    internal class ServiceActorBinder<TService>
         where TService : IServiceActor
    {
        private readonly ServiceActorProviderContext _context;
        private readonly IServiceActorLocator _actorLocator;
        private readonly ISerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAuditLoggerFactory _auditLoggerFactory;
        private readonly Type _serviceType;
        private readonly MethodInfo _dynamicAddGenericMethod;

        public ServiceActorBinder(ServiceActorProviderContext context
            , IServiceActorLocator actorLocator
            , ISerializer serializer
            , ILoggerFactory loggerFactory
            , IAuditLoggerFactory auditLoggerFactory = null
            )
        {
            _serviceType = typeof(TService);
            _context = context;
            _actorLocator = actorLocator;
            _serializer = serializer;
            _loggerFactory = loggerFactory;
            _auditLoggerFactory = auditLoggerFactory;
            _dynamicAddGenericMethod = GetType().GetMethod("AddMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Bind()
        {
            if (!_serviceType.IsInterface)
                return;

            var sAttr = _serviceType.GetCustomAttribute<RpcServiceAttribute>();
            if (sAttr == null)
                return;

            AddRpcService(sAttr);
        }
        private void AddRpcService(RpcServiceAttribute sAttr)
        {
            var methods = _serviceType.GetMethods();
            foreach (var m in methods)
            {
                var mAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (mAttr == null)
                    continue;

                var returnType = m.ReturnType;
                var requestType = m.GetParameters()[0].ParameterType;

                if (!returnType.IsGenericType && returnType.GenericTypeArguments.Length != 1)
                    continue;

                var returnGenericTypes = returnType.GenericTypeArguments[0];
                if (!returnGenericTypes.IsGenericType || returnGenericTypes.GetGenericTypeDefinition() != typeof(RpcResult<>))
                    continue;
                var responseType = returnGenericTypes.GetGenericArguments()[0];

                DynamicAddMethod(m, sAttr, mAttr, requestType, responseType);

            }
        }
        private void DynamicAddMethod(MethodInfo m, RpcServiceAttribute sAttr, RpcMethodAttribute mAttr, Type requestType, Type responseType)
        {

            var dynamicAddMethodInvoker = _dynamicAddGenericMethod.MakeGenericMethod(requestType, responseType);
            var method = new ServerMethod()
            {
                GroupName = sAttr.GroupName,
                ServiceName = _serviceType.Name,
                ServiceId = sAttr.ServiceId,
                MethodId = mAttr.MessageId,
                MethodName = m.Name,
                Handler = m,
            };

            dynamicAddMethodInvoker.Invoke(this, new object[] { method });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>

#pragma warning disable IDE0051 // Remove unused private members
        private void AddMethod<TRequest, TResponse>(ServerMethod method)
#pragma warning restore IDE0051 // Remove unused private members
           where TRequest : class
           where TResponse : class
        {
            var parameters = method.Handler.GetParameters();

            if (parameters?.Length == 1) //without timeout
            {
                var serviceMethod = CreateServiceMethod<ServiceMethod<TService, TRequest, TResponse>, TRequest, TResponse>(method);
                var invoker = new MethodInvoker<TService, TRequest, TResponse>(serviceMethod, null, 0);
                var actorHandler = new ActorCallHandler<TService, TRequest, TResponse>(_actorLocator, invoker, _serializer, _loggerFactory, _auditLoggerFactory);
                var model = new ActorInvokerModel(method, actorHandler.HandleCallAsync);

                _context.AddActorHandler(model);
            }
            else //with timeout
            {
                var serviceMethod = CreateServiceMethod<ServiceMethodWithTimeout<TService, TRequest, TResponse>, TRequest, TResponse>(method);
                var invoker = new MethodInvoker<TService, TRequest, TResponse>(null, serviceMethod, (int)parameters[1].DefaultValue);
                var actorHandler = new ActorCallHandler<TService, TRequest, TResponse>(_actorLocator, invoker, _serializer, _loggerFactory, _auditLoggerFactory);
                var model = new ActorInvokerModel(method, actorHandler.HandleCallAsync);

                _context.AddActorHandler(model);
            }

        }
        private TDelegate CreateServiceMethod<TDelegate, TRequest, TResponse>(
          ServerMethod method
        )
          where TDelegate : Delegate
          where TRequest : class
          where TResponse : class
        {


            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method.Handler);

        }
    }
}
