using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotBPE.Protocol.Amp
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Amp服务端需要的主要注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddAmp(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>()
                    .AddSingleton<IServiceActorLocator<AmpMessage>, ServiceActorLocator>()
                    .AddSingleton<ICallInvoker<AmpMessage>, AmpCallInvoker>()
                    .AddSingleton<IClientProxy, ClientProxy>()
                    .AddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient<AmpMessage>>()
                    .AddSingleton<IRouter<AmpMessage>, LocalPolicyRouter<AmpMessage>>();
        }

        private static IServiceCollection AddAmpClient(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRouter<AmpMessage>)));
            
            return services.AddSingleton<IRouter<AmpMessage>, LoopPolicyRouter<AmpMessage>>()
                    .AddSingleton<IClientMessageHandler<AmpMessage>, ClientMessageHandler<AmpMessage>>() // 消息处理器
                    .AddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory<AmpMessage>>()
                    .AddSingleton<IClientBootstrap<AmpMessage>, NettyClientBootstrap<AmpMessage>>()
                    .AddSingleton<IClientBootstrap, WrapClientBootstrap>(); 
        }

        /// <summary>
        /// 添加服务端依赖，适用于只是服务端，并不依赖其他客户端
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDotBPE(this IServiceCollection services)
        {
            return services.AddServerCore<AmpMessage>() //添加核心依赖
                  .AddNettyServer<AmpMessage>() //使用使用Netty默认实现
                  .AddAmp()
                  .AddAmpClient(); // 使用AMP协议
        }


        /// <summary>
        /// 扫描注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ScanAddServiceActors(this IServiceCollection services, IConfiguration configuration, string dllPrefix, string pluginDirName = "")
        {

            var dllFiles = Directory.GetFiles(string.Concat(BaseDirectory, pluginDirName), $"{dllPrefix}*.dll");
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            //扫描注册所有的ServiceRegistry
            //扫描注册所有的ServiceActor
            var serviceRegistryType = typeof(IServiceDependencyRegistry);
            var serviceActorType = typeof(ServiceActor);
            List<Type> registryTypes = new List<Type>();
            List<Type> actorTypes = new List<Type>();
            foreach (Assembly a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (Type t in a.GetTypes())
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
                ServiceActorDescriptor.AddServiceActor(services, actorTypes);
            }
            return services;
        }
        
    }
}
