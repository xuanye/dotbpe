using System;
using System.Collections.Concurrent;
using Castle.DynamicProxy;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Extra
{
    public class DynamicServiceActorLocator:DefaultServiceActorLocator
    {
        private readonly ConcurrentDictionary<string, IServiceActor<AmpMessage>> DY_ACTOR_CACHE =
            new ConcurrentDictionary<string, IServiceActor<AmpMessage>>();


        private readonly IProxyGenerator _generator;
        private readonly IServiceProvider _provider;

        private IInterceptor _interceptor;

        private IServiceActor<AmpMessage> _proxy_not_found_actor;

        public DynamicServiceActorLocator(IServiceProvider serviceProvider):base(serviceProvider)
        {
            this._provider = serviceProvider;
            this._generator = serviceProvider.GetRequiredService<IProxyGenerator>();
        }



        private IInterceptor ActorInterceptor =>
            this._interceptor ??
            (this._interceptor = this._provider.GetRequiredService<ServiceActorInterceptor>());


        protected override IServiceActor<AmpMessage> GetFromCache(string cacheKey)
        {
            if (DY_ACTOR_CACHE.TryGetValue(cacheKey,out var serviceActor))
            {
                return serviceActor;
            }

            var actor = base.GetFromCache(cacheKey);
            if (actor == null)
            {
                return null;
            }
            var proxy = this._generator.CreateInterfaceProxyWithTarget(actor, ActorInterceptor);
            DY_ACTOR_CACHE.TryAdd(cacheKey, proxy);
            return proxy;
        }

        protected override IServiceActor<AmpMessage> GetNotFoundActor()
        {
            if (_proxy_not_found_actor != null)
            {
                return this._proxy_not_found_actor;
            }

            var actor = base.GetNotFoundActor();
            this._proxy_not_found_actor = this._generator.CreateInterfaceProxyWithTarget(actor, ActorInterceptor);
            return this._proxy_not_found_actor;
        }
    }

}
