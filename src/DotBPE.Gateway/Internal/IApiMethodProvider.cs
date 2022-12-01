// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway.Internal
{
    public interface IApiMethodProvider<TService> where TService : class
    {
        void OnMethodDiscovery(ApiMethodProviderContext<TService> context);
    }
}
