// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Protocols;
using System;
using System.Reflection;

namespace DotBPE.Rpc.Core
{
    public class ServiceActorProvider<TService> : IServiceActorProvider<TService, AmpMessage>
          where TService : class
    {
        private readonly ISerializer _serializer;

        public ServiceActorProvider(ISerializer serializer)
        {
            _serializer = serializer;
        }


        public void OnServiceActorDiscovery(ServiceActorProviderContext<TService> context)
        {
            try
            {

                var binder = new ServiceActorBinder<TService>(context, _serializer);
                binder.Bind();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding RPC service '{typeof(TService).Name}'.", ex);
            }
        }


    }
}
