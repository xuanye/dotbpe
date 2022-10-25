// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client.RoutingPolicies;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using System;

namespace DotBPE.Rpc.Client
{
    public class ClientProxyFactory : IClientProxyFactory
    {
        private readonly IServiceCollection _container;
        private IServiceProvider _provider;
        private ClientProxyFactory(IServiceCollection container)
        {
            _container = container;
        }


        /// <summary>
        /// create client proxy factor
        /// </summary>
        /// <param name="container">if container is null container will be create new instance inside</param>
        /// <returns></returns>
        public static IClientProxyFactory Create(IServiceCollection container = null)
        {
            if (container == null)
            {
                container = new ServiceCollection();
            }

            /* */
            container.AddSingleton<IServiceRouter, DefaultServiceRouter>();
            container.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();

            container.AddSingleton<ICallInvoker, DefaultCallInvoker>();
            container.AddSingleton<IClientMessageHandler, DefaultClientMessageHandler>();
            container.AddSingleton<IRpcClient, DefaultRpcClient>();
            container.AddSingleton<ITransportFactory, DefaultTransportFactory>();

            container.TryAddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();
            container.TryAddSingleton<IRoutingPolicy, RoundrobinRoutingPolicy>();
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
            _container.Configure(configureOptions);
            return this;
        }

        public TService GetService<TService>() where TService : class
        {
            if (_provider == null)
            {
                _provider = _container.BuildServiceProvider();
            }
            return _provider.GetService<TService>();
        }

        /// <summary>
        /// add other dependency services
        /// </summary>
        /// <param name="configServicesDelegate"></param>
        /// <returns></returns>
        public IClientProxyFactory AddDependencyServices(Action<IServiceCollection> configServicesDelegate)
        {
            configServicesDelegate(_container);
            return this;
        }

        /// <summary>
        /// get client proxy instance
        /// </summary>
        /// <returns></returns>
        public IClientProxy GetClientProxy()
        {
            return GetService<IClientProxy>();
        }
    }
}
