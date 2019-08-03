using Microsoft.Extensions.Hosting;

namespace Tomato.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseJsonNetSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddJsonNetSerializer());
        }

    }
}
