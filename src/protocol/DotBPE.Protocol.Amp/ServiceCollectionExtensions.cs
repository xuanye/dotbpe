using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Protocol.Amp
{
    public static class ServiceCollectionExtensions
    {
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
                    .AddSingleton<IClientBootstrap<AmpMessage>, NettyClientBootstrap<AmpMessage>>();
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
        /// 只是客户端，任何服务端
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransforClient(this IServiceCollection services)
        {
            return services.AddClientCore<AmpMessage>()
                 .AddSingleton<ICallInvoker<AmpMessage>, AmpCallInvoker>()
                 .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
        }

        /// <summary>
        /// 添加单网关的
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddGatewayClient(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRouter<AmpMessage>)));
            return services.AddClientCore<AmpMessage>()
                 .AddSingleton<IRouter<AmpMessage>, LoopPolicyRouter<AmpMessage>>()
                 .AddSingleton<ICallInvoker<AmpMessage>, AmpCallInvoker>()
                 .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
        }
    }
}
