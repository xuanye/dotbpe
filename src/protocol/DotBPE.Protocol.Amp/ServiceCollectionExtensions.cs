using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Protocol.Amp
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Amp服务端需要的主键注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAmp(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>()
                    .AddSingleton<IServiceActorLocator<AmpMessage>, ServiceActorLocator>()
                    .AddSingleton<IRpcClient<AmpMessage>, MockRpcClient<AmpMessage>>();
        }

        /// <summary>
        /// 即是全服务端 ，同时又是客户端
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAmpClient(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRpcClient<AmpMessage>)));

            return services.AddSingleton<IRpcClient<AmpMessage>, BridgeRpcClient<AmpMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                    .AddSingleton<IBridgeRouter<AmpMessage>, LocalBridgeRouter<AmpMessage>>() //本地桥接路由器，路由信息在服务启动时添加
                    .AddSingleton<IPreheating, ClientChannelPreheating<AmpMessage>>() //预热
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
