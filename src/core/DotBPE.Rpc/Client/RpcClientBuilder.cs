using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DotBPE.Rpc
{
    public class RpcClientBuilder : IRpcClientBuilder
    {
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;

        private readonly IConfiguration _config;

        public RpcClientBuilder()
        {
            _configureServicesDelegates = new List<Action<IServiceCollection>>();

            _config = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ApplicationName","dotbpe")
            }).Build();
        }

        public IRpcClient<TMessage> Build<TMessage>() where TMessage : InvokeMessage
        {
            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var clientServiceProvider = hostingServices.BuildServiceProvider();

            AddApplicationServices(applicationServices, clientServiceProvider);

            var client = clientServiceProvider.GetRequiredService<IRpcClient<TMessage>>();

            return client;
        }

        public IRpcClientBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IRpcClientBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        private IServiceCollection BuildCommonServices()
        {
            var services = new ServiceCollection();

            // The configured ILoggerFactory is added as a singleton here. AddLogging below will not add an additional one.

            services.AddOptions();

            services.Configure<Options.RpcClientOption>(_config);  // 添加作为客户端的配置

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

           

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

            return services;
        }

        private void AddApplicationServices(IServiceCollection services, IServiceProvider hostingServiceProvider)
        {
        }
    }
}
