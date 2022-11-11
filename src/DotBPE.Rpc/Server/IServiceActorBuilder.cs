// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    public interface IServiceActorBuilder
    {
        void Build();
    }


    public class ServiceActorBuilder : IServiceActorBuilder
    {
        private readonly IServiceProvider _provider;
        private readonly IServiceActorHandlerFactory _actorHandlerFactory;

        public ServiceActorBuilder(IServiceProvider provider, IServiceActorHandlerFactory actorHandlerFactory)
        {
            _provider = provider;
            _actorHandlerFactory = actorHandlerFactory;
        }

        public void Build()
        {
            var actors = _provider.GetServices<IServiceActor>();
            actors.ForEach(actor =>
            {
                BuildServiceActor(actor.GetType());
            });
        }

        private void BuildServiceActor(Type actorType)
        {
            var serviceActorProviderType = typeof(ServiceActorProvider<>);

            var serviceType = actorType.GetInterfaces().FirstOrDefault(x => x.GetCustomAttribute<RpcServiceAttribute>() != null);
            if (serviceType != null)
            {
                var serviceActorProvider = _provider.GetRequiredService(serviceActorProviderType.MakeGenericType(serviceType)) as IServiceActorProvider;
                serviceActorProvider.OnServiceActorDiscovery(new ServiceActorProviderContext(_actorHandlerFactory));
            }

        }
    }
}
