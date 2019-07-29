using Microsoft.Extensions.DependencyInjection;
using Tomato.Rpc;

namespace Tomato.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonNetSerializer(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, JsonSerializer>();
        }

        public static IServiceCollection AddJsonNetParser(this IServiceCollection services)
        {
            return services
                .AddSingleton<IJsonParser, JsonParser>();
        }
    }
}
