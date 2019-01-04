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


        /// <summary>
        /// create client proxy factor
        /// </summary>
        /// <param name="container">if container is null container will be create new instance inside</param>
        /// <returns></returns>
        public static IClientProxyFactory Create(IServiceCollection container =null)
        {
            if (container == null)
            {
                container = new ServiceCollection();
            }

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

        /// <summary>
        /// configure some options
        /// </summary>
        /// <param name="configureOptions"></param>
        /// <typeparam name="TOption"></typeparam>
        /// <returns></returns>
        public IClientProxyFactory Configure<TOption>(Action<TOption> configureOptions) where TOption : class
        {
            this._container.Configure(configureOptions);
            return this;
        }

        /// <summary>
        /// add other dependency services
        /// </summary>
        /// <param name="configServicesDelegate"></param>
        /// <returns></returns>
        public IClientProxyFactory AddDependencyServices(Action<IServiceCollection> configServicesDelegate)
        {
            configServicesDelegate(this._container);
            return this;
        }

        /// <summary>
        /// get client proxy  instance
        /// </summary>
        /// <returns></returns>
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
