// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Abstractions
{
    public interface IServiceActorProvider<TService, AmpMessage>
        where TService : class
    {
        void OnServiceActorDiscovery(ServiceActorProviderContext<TService> context);
    }
}
