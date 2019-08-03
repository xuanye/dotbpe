using Tomato.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tomato.Rpc.Config;
using Tomato.Rpc.Protocol;
using Tomato.Rpc.Server;
using Tomato.Rpc.ServiceDiscovery;
using Peach.Config;


namespace Tomato.Rpc
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseRpcServer(this IHostBuilder builder,string appName="Tomato",int port=5566,
            AddressBindType bindType= AddressBindType.InternalAddress,string specialAddress=null)
        {
            return builder.ConfigureServices(services =>
            {
                services.Configure<RpcServerOptions>(o =>
                {
                    o.Port = port;
                    o.BindType = bindType;
                    o.AppName = appName;
                    //special address logical
                    if (string.IsNullOrEmpty(specialAddress)) return;
                    o.BindType = AddressBindType.SpecialAddress;
                    o.SpecialAddress = specialAddress;
                });
                services.AddScoped<IHostedService, RpcHostedService>();
                services.AddSingleton<IServerBootstrap, AmpTcpServerBootstrap>();
                services.AddTomato();
            });
        }

        #region register services

        public static IHostBuilder BindService<TService>(this IHostBuilder builder)
            where  TService:class,IServiceActor<AmpMessage>
        {
            return builder.ConfigureServices(services => { services.BindService<TService>(); });
        }


        public static IHostBuilder BindServices(this IHostBuilder builder,Action<ServiceActorCollection> serviceConfigureAction)
        {
            return builder.ConfigureServices(services => { services.BindServices(serviceConfigureAction); });
        }

        public static IHostBuilder ScanBindServices(this IHostBuilder builder,string dllPrefix
            ,params string[] groupNames)
        {
            return builder.ConfigureServices((context,services) =>
            {
                services.ScanBindServices(context.Configuration, dllPrefix,true, groupNames);
            });

        }

        public static IHostBuilder ScanBindServices(this IHostBuilder builder,string dllPrefix,bool scanAddDependency
            ,params string[] groupNames)
        {
            return builder.ConfigureServices((context,services) =>
                {
                    services.ScanBindServices(context.Configuration, dllPrefix,scanAddDependency, groupNames);
                });

        }

        public static IHostBuilder UsePort(this IHostBuilder builder, int port)
        {
            return builder.ConfigureServices(services => { services.Configure<RpcServerOptions>(o => o.Port = port); });
        }

        #endregion


        public static Task RunConsoleAsync(this IHostBuilder builder, Action<IHost> configureStartup,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var host = builder.UseConsoleLifetime().Build();
            configureStartup(host);
            return host.RunAsync(cancellationToken);
        }

        public static Task RegisterAndRunConsoleAsync(this IHostBuilder builder,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            //添加默认依赖
            builder.ConfigureServices(services => { services.AddDefaultRegisterService(); });

            return builder.RunConsoleAsync(host =>
            {
                var appLifetime = host.Services.GetRequiredService<IApplicationLifetime>();
                var register = host.Services.GetRequiredService<IServiceRegister>();

                //注册服务
                register.RegisterAllServices().GetAwaiter().GetResult();

                appLifetime.ApplicationStopping.Register(() =>
                {
                    //反注册服务
                    register.DeregisterAllServices().GetAwaiter().GetResult();
                });

            },cancellationToken);
        }


        #region client route policy
        public static IHostBuilder UseRandomPolicy(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddRandomPolicy();
            });
        }

        public static IHostBuilder UseWeightedRoundRobinPolicy(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddWeightedRoundRobinPolicy();
            });
        }
        #endregion
    }
}
