using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    public static class RpcHostBuilderExtensions
    {
        public static IRpcHostBuilder AddRpcCore<TMessage>(this IRpcHostBuilder builder) where TMessage : IMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IMessageHandler<TMessage>, DefaultMessageHandler<TMessage>>()
                    .AddSingleton<IServerHost, DefaultServerHost>(); 
            });
            return builder;
        }
    }
}
