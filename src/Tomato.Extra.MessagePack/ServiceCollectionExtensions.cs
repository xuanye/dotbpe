using Microsoft.Extensions.DependencyInjection;
using Tomato.Rpc;

namespace Tomato.Extra
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
