using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
