using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace DotBPE.Rpc.Hosting
{
    public class RpcHostBuilder:IRpcHostBuilder
    {

        private readonly IHostingEnvironment _env;
        private RpcHostOption _options;

        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;

        private readonly IConfiguration _config;

        private bool _rpcHostBuilt = false;
        public RpcHostBuilder()
        {
            _env = new HostingEnvironment();


            _configureServicesDelegates = new List<Action<IServiceCollection>>();

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


            var host = new DefaultServerHost(hostingServiceProvider,applicationServices,this._options);

            host.Initialize();

            return host;
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

            _env.Initialize(applicationName, _options);


            var services = new ServiceCollection();

            services.AddSingleton(_options);
            services.AddSingleton(_env);

            //类型绑定配置文件
            services.AddOptions();
            services.Configure<Options.RpcClientOption>(_config);  // 添加作为客户端的配置

            services.Configure<Options.RemoteServicesOption>(_config.GetSection("remoteServices"));


            services.AddTransient<IAppBuilder, AppBuilder>();


            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            if (string.IsNullOrEmpty(_options.StartupType))
            {
                services.AddSingleton<IStartup, DefaultStartup>();
            }

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

            return services;
        }

    }
}
