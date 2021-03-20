using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DotBPE.Gateway.Swagger
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {

            services.TryAddSingleton<ISwaggerApiInfoProvider,SwaggerApiInfoProvider>();

            return services;
        }


    }
}
