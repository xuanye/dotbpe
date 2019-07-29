using Microsoft.Extensions.Hosting;

namespace Tomato.Extra
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseProtobufSerializer(this IHostBuilder @this, bool useJsonParser = false)
        {
            return @this.ConfigureServices(services =>
            {
                if (useJsonParser)
                {
                    services.AddProtobufSerializerAndJsonParser();
                }
                else
                {
                    services.AddProtobufSerializer();
                }
            });
        }
    }
}
