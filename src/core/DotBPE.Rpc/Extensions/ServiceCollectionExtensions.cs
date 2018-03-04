using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc
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
        public static IServiceCollection AddClientCore<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            return services.AddSingleton<ITransportFactory<TMessage>, DefaultTransportFactory<TMessage>>()
                   .AddSingleton<IClientMessageHandler<TMessage>, ClientMessageHandler<TMessage>>()
                   .AddSingleton<IRouter<TMessage>, TransforPolicyRouter<TMessage>>()
                   .AddSingleton<IServiceActorLocator<TMessage>, NoopServiceActorLocator<TMessage>>()
               .AddSingleton<IRpcClient<TMessage>, DefaultRpcClient<TMessage>>();
        }

        public static IServiceCollection AddServerCore<TMessage>(this IServiceCollection services) where TMessage : InvokeMessage
        {
            return services.AddSingleton<IServerMessageHandler<TMessage>, ServerMessageHandler<TMessage>>()
                .AddSingleton<IServiceActorContainer<TMessage>, DefaultServiceActorContainer<TMessage>>();
        }

        public static IServiceCollection AddServiceActors<TMessage>(this IServiceCollection services,
           Action<ActorsCollection<TMessage>> actionCollects) where TMessage : InvokeMessage
        {
            var actorsCol = new ActorsCollection<TMessage>();
            actionCollects(actorsCol);
            var actorTypes = actorsCol.GetTypeAll();
            var instances = actorsCol.GetInstanceAll();

            foreach (var actorType in actorTypes)
            {
                services.AddSingleton(typeof(IServiceActor<TMessage>), actorType);
            }
            foreach (var actor in instances)
            {
                services.AddSingleton<IServiceActor<TMessage>>(actor);
            }

            return services;
        }

        public static IServiceCollection AddServiceActor<TActor, TMessage>(this IServiceCollection services)
        where TMessage : InvokeMessage where TActor : class, IServiceActor<TMessage>
        {
            return services.AddSingleton<IServiceActor<TMessage>, TActor>();
        }
    }
}
