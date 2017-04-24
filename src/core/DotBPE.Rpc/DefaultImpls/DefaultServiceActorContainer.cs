using System;
using System.Collections.Generic;
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
        public DefaultServiceActorContainer(IServiceProvider provider){
            InitFromProvider(provider);
        }

        private void InitFromProvider(IServiceProvider provider)
        {
           var actorList =  provider.GetServices<IServiceActor<TMessage>>();
           if(actorList !=null){
               lock(lockObj){
                    foreach(var actor in actorList){

                        if(!actorDict.ContainsKey(actor.Id))
                        {
                            actorDict.Add(actor.Id,actor);
                        }
                        else{
                            Logger.Error("Same Actor，Id={0},Type={1}",actor.Id,actor.GetType());
                        }

                    }
               }

           }
        }

        public IServiceActor<TMessage> GetById(string actorId)
        {
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
            return null;
        }
    }
}
