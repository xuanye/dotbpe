using Microsoft.Extensions.Hosting;

namespace Tomato.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseMessagePackSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddMessagePackSerializer());
        }

    }
}
