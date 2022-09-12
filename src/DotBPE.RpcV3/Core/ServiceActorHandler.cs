// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocols;
using Peach;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Core
{
    public class ServiceActorHandler : IServiceActorHandler
    {
        private readonly IServiceActor _serviceActor;
        private readonly ActorInvokerModel _invoker;

        public ServiceActorHandler(IServiceActor serviceActor, ActorInvokerModel invoker)
        {
            _serviceActor = serviceActor;
            _invoker = invoker;
        }



        public Task HandleAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            return _invoker.RequestDelegate(_serviceActor, context, message);
        }
    }
}
