using Microsoft.Extensions.Configuration;

namespace DotBPE.Plugin.Consul.Config
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder,ConsulConfigurationOptions Options){
            return builder.Add(new ConsulConfigurationSource(Options));
        }
    }
}
