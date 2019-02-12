using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProtobufSerializer(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, ProtobufSerializer>();

        }

        public static IServiceCollection AddProtobufSerializerAndJsonParser(this IServiceCollection services)
        {
            services.TryAddSingleton<IJsonParser, JsonParser>();
            return services
                .AddSingleton<ISerializer, ProtobufSerializer>(); 
        }
    }
}
