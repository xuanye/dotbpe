using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.RouterPolicy;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using Peach.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotBPE(this IServiceCollection services)
        {
            services.AddAmpProtocol();
            services.AddDefaultImpl();
            return services;
        }

        private static IServiceCollection AddAmpProtocol(this IServiceCollection services)
        {
            services.AddSingleton<IProtocol<AmpMessage>, AmpProtocol>();
            services.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();
            services.AddSingleton<ISocketService<AmpMessage>, AmpRpcService>();
            return services;
        }

        private static IServiceCollection AddDefaultImpl(this IServiceCollection services)
        {
            //sever
            services.TryAddSingleton<IServiceActorLocator<AmpMessage>, DefaultServiceActorLocator>();
            services.TryAddSingleton<IServerMessageHandler<AmpMessage>, DefaultServerMessageHandler>();

            //client
            services.TryAddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient>();
            services.TryAddSingleton<ICallInvoker, DefaultCallInvoker>();
            services.TryAddSingleton<IClientMessageHandler<AmpMessage>, DefaultClientMessageHandler>();
            services.TryAddSingleton<IRouterPolicy, RoundrobinPolicy>();
            services.TryAddSingleton<IServiceRouter, DefaultServiceRouter>();
            services.TryAddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory>();

            return services;
        }
    }
}
