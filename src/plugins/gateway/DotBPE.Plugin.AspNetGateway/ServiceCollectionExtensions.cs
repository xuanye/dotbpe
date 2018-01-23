using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Plugin.AspNetGateway
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加转发器相关的注入服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransforClient<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            // .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>() // 编解码
            return services.AddSingleton<ITransportFactory<TMessage>, DefaultTransportFactory<TMessage>>() //通道工厂
                .AddSingleton<IMessageHandler<TMessage>, ClientMessageHandler<TMessage>>() // 消息处理器
                .AddSingleton<IRpcClient<TMessage>, TransforRpcClient<TMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                .AddSingleton<IBridgeRouter<TMessage>, LocalBridgeRouter<TMessage>>() //桥接路由器
                .AddSingleton<IClientBootstrap<TMessage>, NettyClientBootstrap<TMessage>>();
        }

        /// <summary>
        /// 添加本地代理模式客户端
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAgentClient<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            // .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>() // 编解码
            return services.AddSingleton<IMessageHandler<TMessage>, ClientMessageHandler<TMessage>>() // 消息处理器
                .AddSingleton<IRpcClient<TMessage>, AgentRpcClient<TMessage>>(); //在服务本地启动 HTTP Gateway时需要添加该实现        
            
        }
    }
}
