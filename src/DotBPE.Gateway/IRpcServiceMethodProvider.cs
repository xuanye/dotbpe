using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway
{
    public interface IRpcServiceMethodProvider<TService> where TService : class
    {
        void OnServiceMethodDiscovery(RpcServiceMethodProviderContext<TService> context);
    }
}
