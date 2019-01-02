using System;
using DotBPE.Rpc.Client.RouterPolicy;
using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using Peach.Protocol;

namespace DotBPE.Rpc.Client
{
    public class ClientProxyFactory:IClientProxyFactory
    {
        private readonly IServiceCollection _container;
        private IServiceProvider _provider;
        private ClientProxyFactory(IServiceCollection container)
        {
            this._container = container;
        }

        public static IClientProxyFactory Create()
        {
            var container = new ServiceCollection();
            return Create(container);
        }

        public static IClientProxyFactory Create(IServiceCollection container)
        {
            container.AddSingleton<IProtocol<AmpMessage>, AmpProtocol>();
            container.AddSingleton<IServiceRouter, DefaultServiceRouter>();
            container.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();
            container.AddSingleton<IServiceActorLocator<AmpMessage>, DefaultServiceActorLocator>();
            container.AddSingleton<ICallInvoker, DefaultCallInvoker>();
            container.AddSingleton<IClientMessageHandler<AmpMessage>, DefaultClientMessageHandler>();
            container.AddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient>();
            container.AddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory>();
            container.TryAddSingleton<IClientAuditLoggerFactory,DefaultClientAuditLoggerFactory>();
            container.TryAddSingleton<IRouterPolicy, RoundrobinPolicy>();
            container.Configure<RpcClientOptions>(x => { });
            container.AddLogging();
            container.AddOptions();
            return new ClientProxyFactory(container);
        }

        public IClientProxyFactory Configure<TOption>(Action<TOption> configureOptions) where TOption : class
        {
            this._container.Configure(configureOptions);
            return this;
        }

        public IClientProxyFactory AddDependencyServices(Action<IServiceCollection> configServicesDelegate)
        {
            configServicesDelegate(this._container);
            return this;
        }
        public IClientProxy GetProxyInstance()
        {
            if (this._provider == null)
            {
                this._provider = this._container.BuildServiceProvider();
            }
            return this._provider.GetRequiredService<IClientProxy>();
        }
    }
}
