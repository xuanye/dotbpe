// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;

namespace DotBPE.Extra
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseCastleDynamicClientProxy(this IClientProxyFactory @this)
        {
            return @this.AddDependencyServices(services => { services.AddDynamicClientProxy(); });
        }

    }
}
