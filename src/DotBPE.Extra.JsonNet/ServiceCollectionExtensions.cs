using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;

namespace DotBPE.Extra
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
