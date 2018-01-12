using DotBPE.Rpc.Codes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotBPE.Rpc
{
    public static class IServiceCollectionExtension
    {
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
