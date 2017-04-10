using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    public static class RpcClientBuilderExtensions
    {
        public static IRpcClientBuilder AddCore<TMessage>(this IRpcClientBuilder builder) where TMessage:InvokeMessage
        {
            
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<ITransportFactory<TMessage>,DefaultTransportFactory<TMessage>>()
                    .AddSingleton<IMessageHandler<TMessage>>(new ClientMessageHandler<TMessage>())
                .AddSingleton<IRpcClient<TMessage>,DefaultRpcClient<TMessage>>();
            });
            return builder;
        }
    }
}
