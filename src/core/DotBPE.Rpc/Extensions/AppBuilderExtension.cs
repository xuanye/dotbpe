using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    public static class AppBuilderExtension
    {
        /*
        public static IAppBuilder UseBpe<TMessage>(this IAppBuilder builder) where TMessage:InvokeMessage
        {
            var container = builder.ServiceProvider.GetRequiredService<IServiceActorContainer<TMessage>>();
            container.Initialize(builder.ServiceProvider);
            return builder;
        }
        */
    }
}
