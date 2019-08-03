using System;
using System.Collections.Generic;
using Consul;
using Tomato.Rpc.Client;
using Microsoft.Extensions.Hosting;

namespace Tomato.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseConsulServiceRegistration(this IHostBuilder @this
            ,Action<IRouterPoint,List<AgentServiceCheck>> serviceCheckAction =null)
        {
            return @this.ConfigureServices((context,services) =>
            {
                services.AddConsulOptions(context.Configuration);
                services.AddConsulServiceRegistration(serviceCheckAction);
            });
        }

        public static IHostBuilder UseConsulDnsServiceDiscovery(this IHostBuilder @this)
        {
            return @this.ConfigureServices((context,services) =>
            {
                services.AddConsulOptions(context.Configuration);
                services.AddConsulDnsServiceDiscovery();
            });
        }

        public static IHostBuilder UseConsulDnsServiceDiscovery(this IHostBuilder @this,Action<ConsulOptions> configAction)
        {
            return @this.ConfigureServices(services =>
            {
                services.AddConsulOptions(configAction);
                services.AddConsulDnsServiceDiscovery();
            });
        }

    }
}
