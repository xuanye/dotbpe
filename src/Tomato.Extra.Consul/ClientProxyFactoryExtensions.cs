using System;
using Tomato.Rpc;
using Tomato.Rpc.Client;
using Microsoft.Extensions.Configuration;

namespace Tomato.Extra
{
    public static class ClientProxyFactoryExtensions
    {
        public static IClientProxyFactory UseConsulDnsServiceDiscovery(this IClientProxyFactory @this,IConfiguration configuration)
        {
            return @this
                .AddDependencyServices(services =>
            {
                services.AddConsulOptions(configuration);
                services.AddConsulDnsServiceDiscovery();
            });
        }


        public static IClientProxyFactory UseConsulDnsServiceDiscovery(this IClientProxyFactory @this,
            Action<ConsulOptions> configAction =null)
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
