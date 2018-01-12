using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Netty
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNettyServer<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            return services.AddSingleton<IServerBootstrap, NettyServerBootstrap<TMessage>>();
        }
    }
}
