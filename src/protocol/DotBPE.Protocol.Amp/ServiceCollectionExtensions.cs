using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using DotBPE.Rpc.Server;
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
        public static IServiceCollection AddAmp(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>()
                    .AddSingleton<IServiceActorLocator<AmpMessage>, ServiceActorLocator>()
                    .AddSingleton<ICallInvoker<AmpMessage>, AmpCallInvoker>()
                    .AddSingleton<ClientProxy>()
                    .AddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient<AmpMessage>>()
                    .AddSingleton<IRouter<AmpMessage>, LocalPolicyRouter<AmpMessage>>();
        }

        /// <summary>
        /// 即是服务端 ，同时又是客户端
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAmpClient(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRouter<AmpMessage>)));

            return services.AddSingleton<IRouter<AmpMessage>, LoopPolicyRouter<AmpMessage>>()
                    .AddSingleton<IClientMessageHandler<AmpMessage>, ClientMessageHandler<AmpMessage>>() // 消息处理器          
                    .AddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory<AmpMessage>>()
                    .AddSingleton<IClientBootstrap<AmpMessage>, NettyClientBootstrap<AmpMessage>>();
        }

        /// <summary>
        /// 服务端核心协议
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDotBPE(this IServiceCollection services)
        {
            return services.AddRpcCore<AmpMessage>() //添加核心依赖
                    .AddNettyServer<AmpMessage>() //使用使用Netty默认实现
                    .AddAmp(); // 使用AMP协议

        }
    }
}
