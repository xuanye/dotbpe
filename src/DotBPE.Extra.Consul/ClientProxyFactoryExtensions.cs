// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using Microsoft.Extensions.Configuration;
using System;

namespace DotBPE.Extra
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseConsulDnsServiceDiscovery(this IClientProxyFactory @this, IConfiguration configuration)
        {
            return @this
                .AddDependencyServices(services =>
            {
                services.AddConsulOptions(configuration);
                services.AddConsulDnsServiceDiscovery();
            });
        }


        public static IClientProxyFactory UseConsulDnsServiceDiscovery(this IClientProxyFactory @this,
            Action<ConsulOptions> configAction = null)
        {
            return @this
                .AddDependencyServices(services =>
                {
                    services.AddDiscoveryServiceRouter();
                    services.AddConsulOptions(configAction);
                    services.AddConsulDnsServiceDiscovery();
                });
        }

    }
}
