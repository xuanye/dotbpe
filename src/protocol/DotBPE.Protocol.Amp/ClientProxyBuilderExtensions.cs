using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Protocol.Amp
{
    public static class ClientProxyBuilderExtensions
    {
        public static IClientProxy BuildDefault(this IClientProxyBuilder builder)
        {
            var client = builder
                .AddCore<AmpMessage>()             
                .UserNettyClient<AmpMessage>()
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<ICallInvoker<AmpMessage>, AmpCallInvoker>();
                    services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();
                })  
                .Build();

            return client;
        }
    }
}
