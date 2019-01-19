using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiggyMetrics.Common.Consul.Service;
using PiggyMetrics.Common.Extension;
using Vulcan.DataAccess;
using Vulcan.DataAccess.Context;

namespace PiggyMetrics.Common
{
    public abstract class StartupBase:IStartup
    {

        protected StartupBase()
        {

            _localConfiguration = LocalConfig.Load();



            if (string.IsNullOrEmpty(_localConfiguration.ConsulServer))
            {
                throw new Exception("consul.sever 未配置");
            }

            var consulOptions = new ConsulConfigurationOptions
            {
                Key = _localConfiguration.AppName,
                ConsulAddress = new Uri(_localConfiguration.ConsulServer)
            };


            var builder = new ConfigurationBuilder()
                .AddConsul(consulOptions);

            Configuration = builder.Build();
        }

        private readonly LocalConfig _localConfiguration;

        public IConfiguration Configuration { get; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDotBPE();

            // 不依赖其他服务
            if(!string.IsNullOrEmpty(_localConfiguration.RequireService)){
                services.AddAmpServerConsulClient(); //即是服务端又是客服端
                services.AddSingleton<IServiceDiscovery>(new ConsulServiceDiscovery(_localConfiguration.AppName,_localConfiguration.RequireService, (config) =>
                {
                    config.Address = new Uri(_localConfiguration.ConsulServer);
                }));
            }


            services.AddServiceActors<AmpMessage>(AddServiceActors);

            services.AddSingleton<IServiceRegistration>(new ConsulServiceRegistration(_localConfiguration.AppName, (config) =>
            {
                config.Address = new Uri(_localConfiguration.ConsulServer);
            }));

            services.Configure<DbOption>(Configuration.GetSection("DbOption"));


            //注册DB
            AddBizServices(services);

            services.AddSingleton<IContextAccessor<AmpMessage>, DefaultContextAccessor<AmpMessage>>();
            services.AddSingleton<IRuntimeContextStorage, DotBPECallContextStorage>();
            services.AddSingleton<IConnectionFactory, MySqlConnectionFactory>();
            //还要设置数据库链接 不然没法启动

            return services.BuildServiceProvider();
        }

        protected abstract void AddServiceActors(ActorsCollection<AmpMessage> services);
        protected abstract void AddBizServices(IServiceCollection services);

        public void Configure(IAppBuilder app, IHostingEnvironment env)
        {
            //设置数据库连接存储器
            AppRuntimeContext.Configure(app.ServiceProvider.GetRequiredService<IRuntimeContextStorage>());

            //设置使用哪种类型的数据库
            ConnectionFactoryHelper.Configure(app.ServiceProvider.GetRequiredService<IConnectionFactory>());


            app.UseConsulRegistration(Constants.SERVICE_NAME,_localConfiguration.AppName, _localConfiguration.Address, _localConfiguration.Port);

            if(!string.IsNullOrEmpty(_localConfiguration.RequireService)){
                app.UseConsuleDiscovery();
            }
        }
    }
}
