using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Netty;
using DotBPE.Rpc.DefaultImpls;

namespace DotBPE.Protocol.Amp
{
    public static class RpcHostBuilderExtensions
    {
        public static IRpcHostBuilder UseAmp(this IRpcHostBuilder builder)
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>()
                    .AddSingleton<IServiceActorLocator<AmpMessage>, ServiceActorLocator>();
            });
            return builder;
        }

        /// <summary>
        /// 即是全服务端 ，同时又是客户端
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRpcHostBuilder UseAmpClient(this IRpcHostBuilder builder){

            builder.ConfigureServices((services)=>{

                services.AddSingleton<IRpcClient<AmpMessage>,BridgeRpcClient<AmpMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                    .AddSingleton<IBridgeRouter<AmpMessage>,AmpBridgeRouter>() //桥接路由器
                    .AddSingleton<IPreheating,ClientChannelPreheating>() //预热
                    .AddSingleton<ITransportFactory<AmpMessage>,DefaultTransportFactory<AmpMessage>>()
                    .AddSingleton<IClientBootstrap<AmpMessage>,NettyClientBootstrap<AmpMessage>>();
            });

            return builder;
        }

        public static IRpcHostBuilder AddServiceActor(this IRpcHostBuilder builder,params IServiceActor<AmpMessage>[] actors)
        {
            foreach(var actor in actors)
            {
                SimpleServiceActorFactory.RegisterServiceActor(actor);
            }
            return builder;
        }
    }
}
