// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace DotBPE.Extra
{
    public class DynamicServiceActorLocator : DefaultServiceActorLocator
    {
        private readonly ConcurrentDictionary<string, IServiceActor> _actorCache =
            new ConcurrentDictionary<string, IServiceActor>();

        private readonly IProxyGenerator _generator;
        private readonly Interceptor[] _actorInterceptors;

        private IServiceActor _proxyNotFoundActor;

        public DynamicServiceActorLocator(IProxyGenerator generator
            , IEnumerable<Interceptor> actorInterceptors
            , IEnumerable<IServiceActor> serviceActors
            , ILoggerFactory loggerFactory)
            : base(serviceActors, loggerFactory)
        {
            _generator = generator;
            _actorInterceptors = actorInterceptors.ToArray();
        }

        protected override IServiceActor GetFromCache(string cacheKey)
        {
            if (_actorCache.TryGetValue(cacheKey, out var serviceActor))
            {
                return serviceActor;
            }

            var actor = base.GetFromCache(cacheKey);
            if (actor == null)
            {
                return null;
            }

            var interfaces = Array.FindAll(actor.GetType().GetInterfaces(), x => x != typeof(IServiceActor));

            var proxy = (IServiceActor)_generator.CreateInterfaceProxyWithTarget(
                typeof(IServiceActor)
                , interfaces.ToArray()
                , actor
                , _actorInterceptors);

            //var proxy = this._generator.CreateInterfaceProxyWithTarget(actor, ActorInterceptor);
            _actorCache.TryAdd(cacheKey, proxy);
            return proxy;
        }

        protected override IServiceActor GetNotFoundActor()
        {
            if (_proxyNotFoundActor != null)
            {
                return _proxyNotFoundActor;
            }

            var actor = base.GetNotFoundActor();
            _proxyNotFoundActor = _generator.CreateInterfaceProxyWithTarget(actor, _actorInterceptors);
            return _proxyNotFoundActor;
        }
    }
}