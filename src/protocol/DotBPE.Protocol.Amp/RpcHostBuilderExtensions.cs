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
    public static class RpcHostBuilderExtensions
    {
        public static IRpcHostBuilder UseAmp(this IRpcHostBuilder builder)
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>()
                    .AddSingleton<IServiceActorLocator<AmpMessage>, ServiceActorLocator>()
                    .AddSingleton<IRpcClient<AmpMessage>,MockRpcClient<AmpMessage>>()
                    .AddSingleton<IServiceActorContainer<AmpMessage>,DefaultServiceActorContainer<AmpMessage>>();
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

                services.Remove(ServiceDescriptor.Singleton(typeof(IRpcClient<AmpMessage>)));

                services.AddSingleton<IRpcClient<AmpMessage>,BridgeRpcClient<AmpMessage>>() //在服务端使用客户端链接 需要使用桥接式的实现
                    .AddSingleton<IBridgeRouter<AmpMessage>,AmpBridgeRouter>() //桥接路由器
                    .AddSingleton<IPreheating,ClientChannelPreheating>() //预热
                    .AddSingleton<ITransportFactory<AmpMessage>,DefaultTransportFactory<AmpMessage>>()
                    .AddSingleton<IClientBootstrap<AmpMessage>,NettyClientBootstrap<AmpMessage>>();
            });

            return builder;
        }

        public static IRpcHostBuilder AddServiceActors(this IRpcHostBuilder builder,
            Action<ActorsCollection> actionCollects)
        {
            ActorsCollection actorsCol = new ActorsCollection();
            actionCollects(actorsCol);
            var actorTypes = actorsCol.GetAll();
            builder.ConfigureServices(services =>
            {
                foreach (var actorType in actorTypes)
                {
                    services.AddSingleton(typeof(IServiceActor<AmpMessage>),actorType);
                }

            });

            return builder;
        }

        public static IRpcHostBuilder AddServiceActor<TActor>(this IRpcHostBuilder builder
        ) where TActor: class,IServiceActor<AmpMessage>
        {

            builder.ConfigureServices((services)=>{
                services.AddSingleton<IServiceActor<AmpMessage>,TActor>();

            });

            return builder;
        }
    }

    public class ActorsCollection
    {
        private readonly List<Type> _list;
        public ActorsCollection()
        {
            _list = new List<Type>();
        }
        public ActorsCollection Add<TActor>() where TActor : class, IServiceActor<AmpMessage>
        {
            if (!_list.Contains(typeof(TActor)))
            {
                _list.Add(typeof(TActor));
            }
            return this;
        }

        public List<Type> GetAll()
        {
            return _list;
        }
    }
}
