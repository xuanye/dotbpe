// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Abstractions
{
    public interface IServiceActorHandlerFactory
    {
        IServiceActorHandler GetInstance(string methodIdentifier);
        void RegisterActorInvokerHandler(ActorInvokerModel actorModel);
    }
}
