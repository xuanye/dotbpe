using DotBPE.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Hosting
{
    public static class HostingHostBuilderExtensions
    {

    

        /// <summary>
        /// 使用单独的配置信息
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IHostBuilder UseSetting(this IHostBuilder hostBuilder, string key,string value)
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

        public static IHostBuilder UseServer(this IHostBuilder builder, string ip, int port)
        {
            return builder.UseServer(string.Format("{0}:{1}", ip, port));          
        }

        public static IHostBuilder UseServer(this IHostBuilder builder, string address)
        {
            return builder.UseSetting(HostDefaultKey.HOSTADDRESS_KEY, address);
        }

       
    }
}
