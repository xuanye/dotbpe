using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotBPE.Gateway
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddGateway(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpMetricFactory,DefaultHttpMetricFactory>();
            return services;
        }
    }
}
