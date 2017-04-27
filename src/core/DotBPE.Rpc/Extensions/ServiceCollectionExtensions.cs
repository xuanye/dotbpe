using DotBPE.Rpc.Codes;
using DotBPE.Rpc.DefaultImpls;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Extensions
{
    internal static class ServiceCollectionExtensionsInner
    {
        public static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in serviceCollection)
            {
                clone.Add(service);
            }
            return clone;
        }

    }

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRpcCore<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            return services.AddSingleton<IMessageHandler<TMessage>, DefaultMessageHandler<TMessage>>()
                .AddSingleton<IServiceActorContainer<TMessage>,DefaultServiceActorContainer<TMessage>>();
        }
    }
}
