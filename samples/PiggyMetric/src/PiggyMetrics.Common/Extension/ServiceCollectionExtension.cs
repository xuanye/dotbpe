using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;

namespace PiggyMetrics.Common.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAmpServerConsulClient(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRpcClient<AmpMessage>)));

            return services.AddSingleton<IRpcClient<AmpMessage>, BridgeRpcClient<AmpMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                .AddSingleton<IBridgeRouter<AmpMessage>, AmpBridgeConsulRouter>() //桥接路由器
                .AddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory<AmpMessage>>()
                .AddSingleton<IClientBootstrap<AmpMessage>, NettyClientBootstrap<AmpMessage>>();
        }
        public static IServiceCollection AddAmpConsulClient(this IServiceCollection services)
        {
            return  services.AddSingleton<ITransportFactory<AmpMessage>,DefaultTransportFactory<AmpMessage>>() //通道工厂
                .AddSingleton<IMessageHandler<AmpMessage>,ClientMessageHandler<AmpMessage>>() // 消息处理器
                .AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>() // 编解码
                .AddSingleton<IRpcClient<AmpMessage>, TransforRpcClient>() //在服务端使用客户端链接 需要使用桥接式的实现
                .AddSingleton<IBridgeRouter<AmpMessage>, AmpBridgeConsulRouter>() //桥接路由器
                .AddSingleton<IClientBootstrap<AmpMessage>, NettyClientBootstrap<AmpMessage>>();
        }
    }
}
