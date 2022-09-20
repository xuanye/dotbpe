﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public interface IServiceActorProvider<TService>
        where TService : IServiceActor
    {
        void OnServiceActorDiscovery(ServiceActorProviderContext context);
    }
}
