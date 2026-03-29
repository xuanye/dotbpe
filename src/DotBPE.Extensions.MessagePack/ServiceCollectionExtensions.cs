using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;

namespace DotBPE.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessagePackSerializer(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, MessagePackSerializer>();


        }
    }
}
