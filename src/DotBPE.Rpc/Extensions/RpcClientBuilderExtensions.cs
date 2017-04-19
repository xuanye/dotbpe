using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Utils;
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
        public static IRpcClientBuilder UseServer(this IRpcClientBuilder builder ,string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), "服务器地址不能为空");
            builder.UseSetting("DefaultServerAddress", remoteAddress);
            return builder;
        }
    }
}
