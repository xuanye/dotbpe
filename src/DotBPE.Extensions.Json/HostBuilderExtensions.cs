using Microsoft.Extensions.Hosting;

namespace DotBPE.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseJsonNetSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddTextJsonSerializer());
        }

    }
}
