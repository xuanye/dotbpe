using Microsoft.Extensions.Hosting;

namespace DotBPE.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCastleDynamicProxy(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services =>
            {
                services.AddDynamicClientProxy().AddDynamicServiceProxy();
            });
        }

        public static IHostBuilder UseCastleDynamicClientProxy(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => { services.AddDynamicClientProxy(); });
        }
        public static IHostBuilder UseCastleDynamicServiceProxy(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => { services.AddDynamicServiceProxy(); });
        }

    }
}
