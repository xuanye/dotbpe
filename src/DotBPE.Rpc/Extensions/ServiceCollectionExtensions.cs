// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.Impl;
using DotBPE.Rpc.Client.RoutingPolicies;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

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
                if (type.IsAssignableFrom(actorType) && type.IsClass)
                {
                    var serviceAttr = type.GetCustomAttribute<RpcServiceAttribute>(true);
                    if (serviceAttr == null)
                    {
                        continue;
                    }
                    if (groups.Contains(serviceAttr.GroupName))
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
                if (type.IsAssignableFrom(actorType) && type.IsClass)
                {

                    var serviceAttr = type.GetCustomAttribute<RpcServiceAttribute>(true);
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


        #region  Private Method
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

        #endregion
    }
}
