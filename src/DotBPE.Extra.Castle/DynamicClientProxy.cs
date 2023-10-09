// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class DynamicClientProxy : IClientProxy
    {
        private readonly IProxyGenerator _generator;
        private readonly IInterceptor _remoteInvoker;
        private readonly IServiceProvider _provider;
        private readonly IServiceRouter _serviceRouter;

        private IServiceActorLocator _actorLocator;
        private ClientInterceptor[] _clientInterceptors;

        private readonly ConcurrentDictionary<string, object> _typeCache = new ConcurrentDictionary<string, object>();

        public DynamicClientProxy(IProxyGenerator generator
            , IServiceRouter serviceRouter
            , RemoteInvokeInterceptor remoteInvoker
            , IServiceProvider provider
            )
        {
            _generator = generator;
            _serviceRouter = serviceRouter;
            _remoteInvoker = remoteInvoker;
            _provider = provider;
        }

        protected IServiceActorLocator ActorLocator => _actorLocator ?? (_actorLocator = _provider.GetService<IServiceActorLocator>());
        protected IEnumerable<ClientInterceptor> ClientInterceptors => _clientInterceptors ?? (_clientInterceptors = _provider.GetServices<ClientInterceptor>().ToArray());

        public TService Create<TService>(ushort specialMessageId = 0) where TService : class
        {
            return CreateAsync<TService>(specialMessageId).GetAwaiter().GetResult();
        }

        public async Task<TService> CreateAsync<TService>(ushort spacialMessageId = 0) where TService : class
        {
            var serviceType = typeof(TService);
            var cacheKey = $"{serviceType.FullName}${spacialMessageId}";

            if (_typeCache.TryGetValue(cacheKey, out var cacheService))
            {
                return (TService)cacheService;
            }

            var service = serviceType.GetCustomAttribute(typeof(RpcServiceAttribute), false);
            if (service == null)
            {
                throw new InvalidOperationException($"Miss RpcServiceAttribute at {serviceType}");
            }
            var sAttr = service as RpcServiceAttribute;

            var serviceIdentity = FormatServiceIdentity(sAttr.GroupName, sAttr.ServiceId, spacialMessageId);
            // $"{sAttr.ServiceId}${spacialMessageId};{sAttr.GroupName}";
            var servicePath = FormatServicePath(sAttr.ServiceId, spacialMessageId);

            var isLocal = await IsLocalCall(serviceIdentity);

            TService proxy;
            if (isLocal)
            {
                var actor = ActorLocator.LocateServiceActor(servicePath);
                if (!(actor is TService realService))
                {
                    throw new InvalidOperationException($"{serviceType.FullName} has no implementation class,should it be configured at remote server");
                }
                proxy = realService;
            }
            else
            {
                var interceptors = new List<IInterceptor>();
                interceptors.AddRange(ClientInterceptors);
                interceptors.Add(_remoteInvoker);
                proxy = _generator.CreateInterfaceProxyWithoutTarget<TService>(interceptors.ToArray());
            }
            _typeCache.TryAdd(cacheKey, proxy);
            return proxy;
        }

        private async Task<bool> IsLocalCall(string serviceIdentity)
        {
            var point = await _serviceRouter.FindRouterPoint(serviceIdentity);
            if (point == null)
            {
                return true;
            }
            return point.RoutePointType == RoutePointType.Local;
        }

        public static string FormatServiceIdentity(string serviceGroupName, int serviceId, ushort messageId)
        {
            return $"{serviceGroupName}.{serviceId}.{messageId}";
        }

        public static string FormatServicePath(int serviceId, ushort messageId)
        {
            return $"{serviceId}.{messageId}";
        }
    }
}