using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTextJsonSerializer(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, TextJsonSerializer>();
        }

        public static IServiceCollection AddTextJsonParser(this IServiceCollection services)
        {
            return services
                .AddSingleton<IJsonParser, TextJsonParser>();
        }
    }
}
