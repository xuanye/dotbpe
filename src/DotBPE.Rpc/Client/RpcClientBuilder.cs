using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace DotBPE.Rpc
{
    public class RpcClientBuilder:IRpcClientBuilder
    {
        private RpcClientOption _options;

        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;
        

        private readonly IConfiguration _config;
       
       
        public RpcClientBuilder()
        {
            _configureServicesDelegates = new List<Action<IServiceCollection>>();
          

            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DotRPC_")
                .Build();

        }


        public IRpcClient<TMessage> Build<TMessage>() where TMessage :InvokeMessage
        {
           
            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var clientServiceProvider = hostingServices.BuildServiceProvider();

            AddApplicationServices(applicationServices, clientServiceProvider);

            var client = clientServiceProvider.GetRequiredService<IRpcClient<TMessage>>();
            Environment.SetServiceProvider(clientServiceProvider);
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
            _options = new RpcClientOption(_config);

            var services = new ServiceCollection();

            services.AddSingleton(_options);
            // The configured ILoggerFactory is added as a singleton here. AddLogging below will not add an additional one.
            
            services.AddSingleton<IConfiguration>(_config);

         

            var listener = new DiagnosticListener("DotRPC");
            services.AddSingleton<DiagnosticListener>(listener);
            services.AddSingleton<DiagnosticSource>(listener);




            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();


            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

            return services;
        }

        private void AddApplicationServices(IServiceCollection services, IServiceProvider hostingServiceProvider)
        {
            var listener = hostingServiceProvider.GetService<DiagnosticListener>();
            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticListener), listener));
            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticSource), listener));
        }
    }
}
