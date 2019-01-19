using Microsoft.Extensions.Configuration;

namespace PiggyMetrics.Common
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder,ConsulConfigurationOptions Options){
            return builder.Add(new ConsulConfigurationSource(Options));
        }
    }
}
