using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.RouterPolicy;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach;
using Peach.Tcp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseRpcServer(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddScoped<IHostedService, RpcHostedService>();
                services.AddSingleton<IServerBootstrap, TcpServerBootstrap<AmpMessage>>();
                services.AddDotBPE();
            });            
        }

        #region register services
        public static IHostBuilder BindServices(this IHostBuilder builder, Action<IServiceCollection> configureDelegate)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRouterPolicy, RandomPolicy>();
            });
        }
        #endregion


        #region client route policy
        public static IHostBuilder UseRandomPolicy(this IHostBuilder builder)
        { 
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRouterPolicy, RandomPolicy>();
            });
        }

        public static IHostBuilder UseWeightedRoundRobinPolicy(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRouterPolicy, WeightedRoundRobinPolicy>();
            });
        }
        #endregion
    }
}
