using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace DotBPE.Rpc.Hosting
{
    public static class HostingHostBuilderExtensions
    {
        /// <summary>
        /// 使用单独的配置信息
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Uses the server.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public static IHostBuilder UseServer(this IHostBuilder builder, string ip, int port)
        {
            return builder.UseServer(string.Format("{0}:{1}", ip, port));
        }

        /// <summary>
        /// Uses the server.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static IHostBuilder UseServer(this IHostBuilder builder, string address)
        {
            return builder.UseSetting(HostDefaultKey.HOSTADDRESS_KEY, address);
        }
    }
}
