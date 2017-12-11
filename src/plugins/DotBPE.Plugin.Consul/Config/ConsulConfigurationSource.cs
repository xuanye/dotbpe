using System;
using Consul;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Plugin.Consul.Config
{
    public class ConsulConfigurationSource : IConfigurationSource
    {

        public ConsulConfigurationSource(ConsulConfigurationOptions Options){
            this.Options = Options;
        }
        public ConsulConfigurationOptions Options{get;set;}
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            Action<ConsulClientConfiguration> configOverride =
                Options.ConsulClientConfigOverride ?? new Action<ConsulClientConfiguration>((config) =>
                {
                    config.Address = this.Options.ConsulAddress;
                });

            ConsulClient client = new ConsulClient(configOverride);
            return new ConsulConfigurationProvider(Options,client);
        }

    }
}
