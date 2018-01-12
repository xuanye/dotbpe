using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Netty
{
    public static class RpcClientBuilderExtensions
    {
        public static IRpcClientBuilder UserNettyClient<TMessage>(this IRpcClientBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IClientBootstrap<TMessage>, NettyClientBootstrap<TMessage>>();
            });
            return builder;
        }
    }
}
