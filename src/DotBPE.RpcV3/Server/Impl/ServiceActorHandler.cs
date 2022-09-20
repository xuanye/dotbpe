// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Rpc.Protocols;
using Peach;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class ServiceActorHandler : IServiceActorHandler
    {
   
        private readonly ActorInvokerModel _invoker;

        public ServiceActorHandler(ActorInvokerModel invoker)
        {          
            _invoker = invoker;
        }

        public Task HandleAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            return _invoker.RequestDelegate(context, message);
        }
    }
}
