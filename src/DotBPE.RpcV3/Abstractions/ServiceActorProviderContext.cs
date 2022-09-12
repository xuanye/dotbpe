﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Core;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Abstractions
{
    public class ServiceActorProviderContext<TService>
        where TService : class
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
