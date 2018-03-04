using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Netty
{
    public static class ClientProxyBuilderExtensions
    {
        public static IClientProxyBuilder AddNettyClient<TMessage>(this IClientProxyBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IClientBootstrap<TMessage>, NettyClientBootstrap<TMessage>>();
            });
            return builder;
        }
    }
}
