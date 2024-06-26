﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.Impl;
using DotBPE.Rpc.Client.RoutingPolicies;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the DotBPE service along with the other dependencies needed to enable the RPC service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configServer"></param>
        /// <returns></returns>
        public static IServiceCollection AddDotBPEServer(this IServiceCollection @this, Action<RpcServerOptions> configServer = null)
        {
            if (configServer != null)
            {
                @this.Configure(configServer);
            }
            else
            {
                @this.Configure<RpcServerOptions>(o =>
                {
                    o.Port = RpcServerOptions.Default.Port;
                    o.BindType = RpcServerOptions.Default.BindType;
                });
            }
            return @this.AddDotBPE()
                    .AddPeachServer();
        }

        public static IServiceCollection AddDotBPE(this IServiceCollection @this)
        {
            @this.AddLogging();
            @this.AddOptions();
            @this.AddAmpProtocol();
            @this.AddDefaultImpl();

            return @this;
        }

        public static IServiceCollection AddPeachServer(this IServiceCollection @this)
        {
            //add host service
            @this.AddSingleton<ISocketService<AmpMessage>, AmpPeachSocketService>()
                .AddSingleton<IServerBootstrap, PeachServerBootstrap>()
                .AddSingleton<IServerHost, PeachServerHost>();

            @this.AddHostedService<RpcServiceHostedService>();

            return @this;
        }

        public static IServiceCollection BindService<TService>(this IServiceCollection @this)
            where TService : IServiceActor
        {
            return @this.BindService(typeof(TService));
        }

        public static IServiceCollection BindService(this IServiceCollection @this, Type serviceType)
        {
            return @this.AddSingleton(typeof(IServiceActor), serviceType);
        }

        public static IServiceCollection BindServices(this IServiceCollection @this, Assembly assembly, params string[] groups)
        {
            var actorType = typeof(IServiceActor);
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (actorType.IsAssignableFrom(type) && type.IsClass)
                {
                    var serviceAttr = GetServiceAttribute(type);
                    if (serviceAttr == null)
                    {
                        continue;
                    }
                    if (!groups.Any() || groups.Contains(serviceAttr.GroupName))
                    {
                        @this.BindService(type);
                    }
                }
            }
            return @this;
        }

        public static IServiceCollection BindServices(this IServiceCollection @this, Assembly assembly, Func<ServiceModel, bool> filterFunc = null)
        {
            var actorType = typeof(IServiceActor);
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (actorType.IsAssignableFrom(type) && type.IsClass)
                {
                    var serviceAttr = GetServiceAttribute(type);
                    if (serviceAttr == null)
                    {
                        continue;
                    }
                    var serviceModel = new ServiceModel()
                    {
                        ServiceId = serviceAttr.ServiceId,
                        Group = serviceAttr.GroupName,
                        ServiceType = type
                    };
                    bool add = true;
                    if (filterFunc != null)
                    {
                        add = filterFunc(serviceModel);
                    }
                    if (add)
                    {
                        @this.BindService(type);
                    }
                }
            }
            return @this;
        }

        #region Private Method

        private static IServiceCollection AddAmpProtocol(this IServiceCollection services)
        {
            services.AddSingleton<IChannelHandlerPipeline, AmpChannelHandlerPipeline>();
            services.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();

            return services;
        }

        private static IServiceCollection AddDefaultImpl(this IServiceCollection services)
        {
            //sever
            services.AddSingleton<IServiceActorBuilder, ServiceActorBuilder>();
            services.AddSingleton<IServiceActorHandlerFactory, DefaultServiceActorHandlerFactory>();
            services.AddSingleton(typeof(ServiceActorProvider<>));
            services.AddSingleton<IMessageHandler<AmpMessage>, DefaultMessageHandler>();
            services.TryAddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();

            //client
            services.AddSingleton<IRpcClient, DefaultRpcClient>();
            services.AddSingleton<IMessageSubscriberContainer, DefaultMessageSubscriberContainer>();

            services.AddSingleton<ICallInvoker, DefaultCallInvoker>();
            services.AddSingleton<IClientMessageHandler, DefaultClientMessageHandler>();
            services.AddSingleton<ITransportFactory, DefaultTransportFactory>();

            services.TryAddSingleton<IRoutingPolicy, RoundrobinRoutingPolicy>();
            services.TryAddSingleton<IServiceRouter, DefaultServiceRouter>();

            return services;
        }

        private static RpcServiceAttribute GetServiceAttribute(Type serviceType)
        {
            var formTypes = serviceType.GetInterfaces();
            foreach (var interfaceType in formTypes)
            {
                var serviceAttr = interfaceType.GetCustomAttribute<RpcServiceAttribute>();
                if (serviceAttr != null)
                {
                    return serviceAttr;
                }
            }
            return null;
        }

        #endregion Private Method
    }
}