using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DotBPE.Gateway
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// add gateway
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dllPrefix">service definition dll prefix</param>
        /// <param name="categories">banding route category，default value is ‘default’ category</param>
        /// <returns></returns>
        public static IServiceCollection AddGateway(this IServiceCollection services,string dllPrefix="*"
            ,params string[] categories)
        {
            services.AddDotBPE();
            services.TryAddSingleton<IHttpMetricFactory,DefaultHttpMetricFactory>();
            services.TryAddSingleton<IHttpServiceScanner,HttpServiceScanner>();
            services.ScanRouteOptions(dllPrefix, categories);
            return services;
        }

        private static IServiceCollection ScanRouteOptions(this IServiceCollection services,string dllPrefix="*"
            ,params string[] categories)
        {
            services.AddSingleton<IProtocolProcessor>(p =>
            {
                IJsonParser jsonParser = p.GetRequiredService<IJsonParser>();
                ILogger logger = p.GetRequiredService<ILoggerFactory>().CreateLogger<IProtocolProcessor>();
                IHttpMetricFactory metricFactory = p.GetRequiredService<IHttpMetricFactory>();
                var parsers = p.GetServices<IAdditionalHttpParser>();
                var scanner = p.GetRequiredService<IHttpServiceScanner>();
                var options = scanner.Scan(dllPrefix,categories);
                return new ProtocolProcessor(jsonParser,metricFactory,options, parsers, logger);
            });

            return services;
        }
    }
}
