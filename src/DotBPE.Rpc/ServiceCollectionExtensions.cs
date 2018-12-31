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
using System.IO;
using System.Reflection;
using System.Text;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Rpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotBPE(this IServiceCollection services)
        {
            //add common
            services.AddLogging();
            services.AddOptions();
            services.AddAmpProtocol();
            services.AddDefaultImpl();
            return services;
        }

        public static IServiceCollection BindService<TService>(this IServiceCollection services)
            where  TService:class,IServiceActor<AmpMessage>
        {
            return services.AddSingleton<IServiceActor<AmpMessage>, TService>();
        }

        public static IServiceCollection BindServices(this IServiceCollection services,
            Action<ServiceActorCollection> serviceConfigureAction)
        {
            var list = new ServiceActorCollection();
            serviceConfigureAction(list);


            var actorTypes = list.GetTypeAll();
            var instances = list.GetInstanceAll();

            foreach (var actorType in actorTypes)
            {
                services.AddSingleton(typeof(IServiceActor<AmpMessage>), actorType);
            }
            foreach (var actor in instances)
            {
                services.AddSingleton(actor);
            }

            return services;
        }

        public static IServiceCollection ScanBindServices(this IServiceCollection services,string pluginDirName
            ,IConfiguration configuration,params  string[] categories)
        {
            return services.ScanBindServices(configuration, "*", pluginDirName, categories);
        }
        public static IServiceCollection ScanBindServices(this IServiceCollection services
            ,IConfiguration configuration,string ddlPrefix,params  string[] categories)
        {
            return services.ScanBindServices(configuration, ddlPrefix, "", categories);
        }
        public static IServiceCollection ScanBindServices(this IServiceCollection services,IConfiguration configuration
            ,string dllPrefix,string pluginDirName ,params string[] categories)
        {
            string BaseDirectory = Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(BaseDirectory,pluginDirName), $"{dllPrefix}.dll");
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            //扫描注册所有的ServiceRegistry
            //扫描注册所有的ServiceActor
            var serviceRegistryType = typeof(IServiceDependencyRegistry);
            var serviceActorType = typeof(AbsServiceActor);
            List<Type> registryTypes = new List<Type>();
            List<Type> actorTypes = new List<Type>();
            foreach (Assembly a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var t in a.GetTypes())
                {
                    if (serviceRegistryType.IsAssignableFrom(t) && t.IsClass) //t 实现了某接口
                    {
                        registryTypes.Add(t);
                    }
                    else if (t.IsSubclassOf(serviceActorType) && !t.IsAbstract)
                    {
                        actorTypes.Add(t);
                    }
                }
            }

            if (registryTypes.Count > 0) //注册依赖
            {
                registryTypes.ForEach(r => ServiceActorDescriptor.ServiceDependencyRegistry(configuration, services, r));
            }
            if (actorTypes.Count > 0) //注册服务
            {
                ServiceActorDescriptor.AddServiceActor(services, actorTypes,categories);
            }
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
