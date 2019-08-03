using System;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using Tomato.Rpc.Protocol;
using Tomato.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.Extra
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

            var interfaces = actor.GetType().GetInterfaces().FindAll(x=> x !=typeof(IServiceActor<AmpMessage>));
            //actor 真实的实现实现类 ，actor:IAService,IServiceActor<AmpMessage>
            var proxy = (IServiceActor<AmpMessage>)this._generator.CreateInterfaceProxyWithTarget(
                typeof(IServiceActor<AmpMessage>)
                ,interfaces.ToArray()
                ,actor
                , ActorInterceptor);

            //var proxy = this._generator.CreateInterfaceProxyWithTarget(actor, ActorInterceptor);
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
