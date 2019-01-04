using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonNet(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISerializer, JsonSerializer>();


        }
    }
}
