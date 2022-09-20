// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Protocols;
using System;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    public class ServiceActorProvider<TService> : IServiceActorProvider<TService>
        where TService : IServiceActor
    {
        private readonly IServiceActorLocator _actorLocator;
        private readonly ISerializer _serializer;

        public ServiceActorProvider(IServiceActorLocator actorLocator,ISerializer serializer)
        {
            _actorLocator = actorLocator;
            _serializer = serializer;
        }


        public void OnServiceActorDiscovery(ServiceActorProviderContext context)
        {
            try
            {

                var binder = new ServiceActorBinder<TService>(context, _actorLocator, _serializer);
                binder.Bind();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding RPC service '{typeof(TService).Name}'.", ex);
            }
        }


    }
}
