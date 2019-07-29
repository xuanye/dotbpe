using System;
using System.Collections.Generic;
using System.Net;
using Consul;
using DnsClient;
using Tomato.Rpc.Client;
using Tomato.Rpc.ServiceDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tomato.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulOptions(this IServiceCollection services,
            IConfiguration Configuration)
        {
            services.AddOptions();
            services.Configure<ConsulOptions>(Configuration.GetSection("consul"));

            return services;
        }

        public static IServiceCollection AddConsulOptions(this IServiceCollection services,
            Action<ConsulOptions> configAction = null)
        {
            services.AddOptions();
            if (configAction != null)
            {
                services.Configure(configAction);
            }
            else
            {
                services.Configure<ConsulOptions>(o =>
                {
                    if (o.DnsEndpoint == null)
                    {
                        o.DnsEndpoint = new DnsEndpoint();
                    }
                });
            }

            return services;
        }

        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var options = p.GetRequiredService<IOptions<ConsulOptions>>().Value;

                if (!string.IsNullOrEmpty(options.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(options.HttpEndpoint);
                }
            }));
            return services;
        }

        public static IServiceCollection AddConsulServiceRegistration(this IServiceCollection services
            , Action<IRouterPoint, List<AgentServiceCheck>> serviceCheckAction = null)
        {
            services.AddConsul();
            services.AddSingleton<IServiceRegistrationProvider>(p =>
            {
                var consulClient = p.GetRequiredService<IConsulClient>();
                return new ConsulServiceRegistrationProvider(consulClient, serviceCheckAction);
            });
            return services;
        }

        public static IServiceCollection AddConsulDnsServiceDiscovery(this IServiceCollection services)
        {
            services
                .AddSingleton<IDnsQuery>(p =>
                {
                    var options = p.GetService<IOptions<ConsulOptions>>();
                    var consulOptionValue = options?.Value ?? ConsulOptions.Default;
                    if (consulOptionValue.DnsEndpoint == null)
                    {
                        consulOptionValue.DnsEndpoint = new DnsEndpoint();
                    }
                    return new LookupClient(IPAddress.Parse(consulOptionValue.DnsEndpoint.Address),
                        consulOptionValue.DnsEndpoint.Port){ UseCache = true};
                })
                .AddSingleton<IServiceDiscoveryProvider, ConsulDnsServiceDiscoveryProvider>();
            return services;
        }
    }
}
