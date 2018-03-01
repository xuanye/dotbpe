using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc
{
    public static class RpcClientBuilderExtensions
    {
        public static IRpcClientBuilder AddCore<TMessage>(this IRpcClientBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<ITransportFactory<TMessage>, DefaultTransportFactory<TMessage>>()
                    .AddSingleton<IClientMessageHandler<TMessage>, ClientMessageHandler<TMessage>>()
                    .AddSingleton<IRouter<TMessage>,TransforPolicyRouter<TMessage>>()
                    .AddSingleton<IServiceActorLocator<TMessage>,NoopServiceActorLocator<TMessage>>()
                .AddSingleton<IRpcClient<TMessage>, DefaultRpcClient<TMessage>>();
            });
            return builder;
        }

        public static IRpcClientBuilder UseServer(this IRpcClientBuilder builder, string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), "服务器地址不能为空");
            builder.UseSetting("DefaultServerAddress", remoteAddress);
            return builder;
        }
    }
}
