using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    public static class RpcHostBuilderExtensions
    {
        public static IRpcHostBuilder UseConfiguration(this IRpcHostBuilder builder, IConfiguration config)
        {
            foreach (var setting in config.AsEnumerable())
            {
                builder.UseSetting(setting.Key, setting.Value);
            }

            return builder;
        }

        public static IRpcHostBuilder UseServer(this IRpcHostBuilder builder, string ip, int port)
        {
            builder.UseServer(string.Format("{0}:{1}", ip, port));
            return builder;
        }

        public static IRpcHostBuilder UseServer(this IRpcHostBuilder builder, string address)
        {
            builder.UseSetting(HostDefaultKey.HOSTADDRESS_KEY, address);

            return builder;
        }

        public static IRpcHostBuilder UseStartup<TStartup>(this IRpcHostBuilder builder) where TStartup : IStartup
        {
            var startupType = typeof(TStartup);
            var startupAssemblyName = startupType.FullName;
            return builder.UseSetting(HostDefaultKey.STARTUPTYPE_KEY, startupAssemblyName).ConfigureServices(services =>
            {
                services.AddSingleton(typeof(IStartup), startupType);
            });
        }
    }
}
