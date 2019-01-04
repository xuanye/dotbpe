using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach;
using System;
using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Peach.Config;

namespace DotBPE.Rpc
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseRpcServer(this IHostBuilder builder,int port=5566,
            AddressBindType bindType= AddressBindType.InternalAddress,string specialAddress=null)
        {
            return builder.ConfigureServices(services =>
            {
                services.Configure<RpcServerOptions>(o =>
                {
                    o.Port = port;
                    o.BindType = bindType;

                    //special address logical
                    if (string.IsNullOrEmpty(specialAddress)) return;
                    o.BindType = AddressBindType.SpecialAddress;
                    o.SpecialAddress = specialAddress;
                });
                services.AddScoped<IHostedService, RpcHostedService>();
                services.AddSingleton<IServerBootstrap, AmpTcpServerBootstrap>();
                services.AddDotBPE();
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
                    services.ScanBindServices(context.Configuration, dllPrefix, groupNames);
                });

        }



        #endregion


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
