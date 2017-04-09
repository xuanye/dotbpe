using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using DotBPE.Rpc.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace DotBPE.Rpc.Hosting
{
    public class RpcHostBuilder:IRpcHostBuilder
    {

        private RpcHostOption _options;

        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates;

        private readonly IConfiguration _config;
        private ILoggerFactory _loggerFactory;
        private bool _rpcHostBuilt = false;
        public RpcHostBuilder()
        {
            _configureServicesDelegates = new List<Action<IServiceCollection>>();
            _configureLoggingDelegates = new List<Action<ILoggerFactory>>();

            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DotRPC_")
                .Build();
         
        }


        public IServerHost Build()
        {
            if (_rpcHostBuilt)
            {
                throw new InvalidOperationException("rpc server is runing");
            }
            _rpcHostBuilt = true;

           

            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var hostingServiceProvider = hostingServices.BuildServiceProvider();

            AddApplicationServices(applicationServices, hostingServiceProvider);

            var host = hostingServiceProvider.GetRequiredService<IServerHost>();

            return host;
        }

        public IRpcHostBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            return this;
        }

        public IRpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IRpcHostBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            if (configureLogging == null)
            {
                throw new ArgumentNullException(nameof(configureLogging));
            }

            _configureLoggingDelegates.Add(configureLogging);
            return this;
        }

        public IRpcHostBuilder UseSetting(string key, string value)
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
            _options = new RpcHostOption(_config);

          
            var applicationName = _options.ApplicationName ?? "DotBPE Application";

        

            var services = new ServiceCollection();

            services.AddSingleton(_options);
            // The configured ILoggerFactory is added as a singleton here. AddLogging below will not add an additional one.
            if (_loggerFactory == null)
            {
                _loggerFactory = new LoggerFactory();
                services.AddSingleton(provider => _loggerFactory);
            }
            else
            {
                services.AddSingleton(_loggerFactory);
            }

            foreach (var configureLogging in _configureLoggingDelegates)
            {
                configureLogging(_loggerFactory);
            }

            //This is required to add ILogger of T.
            services.AddLogging();

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
            
            var loggerFactory = hostingServiceProvider.GetService<ILoggerFactory>();
            services.Replace(ServiceDescriptor.Singleton(typeof(ILoggerFactory), loggerFactory));

            var listener = hostingServiceProvider.GetService<DiagnosticListener>();
            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticListener), listener));
            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticSource), listener));
        }

    }
}
