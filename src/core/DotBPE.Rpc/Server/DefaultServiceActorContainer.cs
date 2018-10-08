using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotBPE.Rpc.Server
{
    public class DefaultServiceActorContainer<TMessage> : IServiceActorContainer<TMessage> where TMessage : InvokeMessage
    {
        private readonly ILogger<DefaultServiceActorContainer<TMessage>> Logger;
        private Dictionary<string, IServiceActor<TMessage>> actorDict = new Dictionary<string, IServiceActor<TMessage>>();
        private static object lockObj = new object();

        public DefaultServiceActorContainer(IServiceProvider serviceProvider, ILogger<DefaultServiceActorContainer<TMessage>> logger)
        {
            this.Logger = logger;

            Initialize(serviceProvider);
        }

        private void Initialize(IServiceProvider serviceProvider)
        {
            var actorList = serviceProvider.GetServices<IServiceActor<TMessage>>();
            if (actorList != null)
            {
                if (actorList.Count() == 0)
                {
                    Logger.LogWarning("no service actors");
                    return;
                }

                lock (lockObj)
                {
                    foreach (var actor in actorList)
                    {
                        if (!actorDict.ContainsKey(actor.Id))
                        {
                            Logger.LogDebug("Register Actor，Id={0},Type={1}", actor.Id, actor.GetType());
                            actorDict.Add(actor.Id, actor);
                        }
                        else
                        {
                            Logger.LogWarning("Same Actor，Id={0},Type={1}", actor.Id, actor.GetType());
                        }
                    }
                }
            }
            else
            {
                Logger.LogWarning("no service actors");
            }
        }

        public IServiceActor<TMessage> GetById(string actorId)
        {
            Logger.LogDebug("Get Actor，Id={actorId}", actorId);

            if (actorDict.ContainsKey(actorId))
            {
                lock (lockObj)
                {
                    if (actorDict.ContainsKey(actorId))
                    {
                        return actorDict[actorId];
                    }
                }
            }
            Logger.LogDebug("Actor not exist，actorId={actorId},Actor Count ={actorCount}", actorId, actorDict.Count);

            return null;
        }
    }
}
