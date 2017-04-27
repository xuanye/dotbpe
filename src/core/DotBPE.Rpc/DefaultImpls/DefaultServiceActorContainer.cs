using System;
using System.Collections.Generic;
using System.Linq;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultServiceActorContainer<TMessage> : IServiceActorContainer<TMessage> where TMessage : InvokeMessage
    {
        static ILogger Logger = Environment.Logger.ForType<DefaultServiceActorContainer<TMessage>>();
        private Dictionary<string, IServiceActor<TMessage>> actorDict = new Dictionary<string, IServiceActor<TMessage>>();
        private static object lockObj = new object();
        public DefaultServiceActorContainer(IServiceProvider serviceProvider){
            Initialize(serviceProvider);
        }

        private void Initialize(IServiceProvider serviceProvider)
        {
           var actorList =  serviceProvider.GetServices<IServiceActor<TMessage>>();
            if(actorList !=null){
                if(actorList.Count() ==0){
                    Logger.Warning("no service actors");
                    return;
                }

                lock(lockObj){
                    foreach(var actor in actorList){

                        if(!actorDict.ContainsKey(actor.Id))
                        {
                            Logger.Debug("Register Actor，Id={0},Type={1}",actor.Id,actor.GetType());
                            actorDict.Add(actor.Id,actor);
                        }
                        else{
                            Logger.Error("Same Actor，Id={0},Type={1}",actor.Id,actor.GetType());
                        }

                    }
                }
            }
            else{
                Logger.Warning("no service actors");
            }
        }

        public IServiceActor<TMessage> GetById(string actorId)
        {
            Logger.Debug("Get Actor，Id={0}",actorId);
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
            Logger.Warning("Actor not exist，Id={0},Actor Count ={1}",actorId,actorDict.Count);

            return null;
        }
    }
}
