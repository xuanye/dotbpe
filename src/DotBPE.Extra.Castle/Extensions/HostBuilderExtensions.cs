using Microsoft.Extensions.Hosting;

namespace DotBPE.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCastleDynamicProxy(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddDynamicClientProxy());
        }

    }
}
