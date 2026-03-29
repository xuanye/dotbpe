using Microsoft.Extensions.Hosting;

namespace DotBPE.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseMessagePackSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddMessagePackSerializer());
        }

    }
}
