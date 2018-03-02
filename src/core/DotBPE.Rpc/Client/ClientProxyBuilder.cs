using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;

namespace DotBPE.Rpc.Client
{
    public class ClientProxyBuilder : IClientProxyBuilder
    {
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;

        private readonly IConfiguration _config;

        public ClientProxyBuilder()
        {
            _configureServicesDelegates = new List<Action<IServiceCollection>>();

            _config = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ApplicationName","dotbpe")
            }).Build();
        }

        public IClientProxy Build()
        {
            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var clientServiceProvider = hostingServices.BuildServiceProvider();

            AddApplicationServices(applicationServices, clientServiceProvider);

            var proxy = clientServiceProvider.GetRequiredService<IClientProxy>();

            Environment.SetServiceProvider(clientServiceProvider);
            var loggerFactory = clientServiceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                Environment.SetLoggerFactory(loggerFactory);
            }

            return proxy;
        }

        public IClientProxyBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IClientProxyBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <param name="configureLogging">The delegate that configures the <see cref="ILoggingBuilder"/>.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IClientProxyBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging)
        {
            return this.ConfigureServices((collection) => collection.AddLogging(builder => configureLogging(builder)));
        }

        private IServiceCollection BuildCommonServices()
        {
            var services = new ServiceCollection();

            // The configured ILoggerFactory is added as a singleton here. AddLogging below will not add an additional one.

            services.AddOptions();

            services.Configure<Options.RpcClientOption>(_config);  // 添加作为客户端的配置

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            services.AddSingleton<IClientProxy, ClientProxy>();

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
