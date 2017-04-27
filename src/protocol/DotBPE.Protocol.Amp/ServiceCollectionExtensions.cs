using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.Rpc.Netty;
using DotBPE.Rpc.DefaultImpls;
using System;
using System.Collections.Generic;

namespace DotBPE.Protocol.Amp
{
    public static class ServiceCollectionExtensions
    {
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

            return services.AddSingleton<IRpcClient<AmpMessage>,BridgeRpcClient<AmpMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                    .AddSingleton<IBridgeRouter<AmpMessage>,AmpBridgeRouter>() //桥接路由器
                    .AddSingleton<IPreheating,ClientChannelPreheating<AmpMessage>>() //预热
                    .AddSingleton<ITransportFactory<AmpMessage>,DefaultTransportFactory<AmpMessage>>()
                    .AddSingleton<IClientBootstrap<AmpMessage>,NettyClientBootstrap<AmpMessage>>();

        }


    }


}
