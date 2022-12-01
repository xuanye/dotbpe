// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Rpc.Server
{
    public class ServiceActorProviderContext 
    {
        private readonly IServiceActorHandlerFactory _handlerFactory;

        public ServiceActorProviderContext(IServiceActorHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }


        public void AddActorHandler(ActorInvokerModel actorModel)
        {
            _handlerFactory.RegisterActorInvokerHandler(actorModel);
        }
    }
}
