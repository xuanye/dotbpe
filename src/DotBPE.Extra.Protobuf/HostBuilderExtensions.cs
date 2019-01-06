using Microsoft.Extensions.Hosting;

namespace DotBPE.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseProtobufSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddProtobufSerializer());
        }

    }
}
