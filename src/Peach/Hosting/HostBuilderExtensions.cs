using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Peach.Config;

namespace Peach.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSetting(this IHostBuilder hostBuilder, string key, string value)
        {
            return hostBuilder.ConfigureHostConfiguration(configBuilder =>
            {
                configBuilder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(key,
                        value  ?? throw new ArgumentNullException(nameof(value)))
                });
            });
        }

        public static IHostBuilder UsePort(this IHostBuilder builder, int port)
        {
            return builder.ConfigureServices(services => { services.Configure<TcpHostOption>(o => o.Port = port); });
        }
    }
}
