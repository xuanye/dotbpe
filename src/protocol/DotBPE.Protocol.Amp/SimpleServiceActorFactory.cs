using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protocol.Amp
{
    public static class SimpleServiceActorFactory
    {
        private static Dictionary<string, IServiceActor<AmpMessage>> actorDict = new Dictionary<string, IServiceActor<AmpMessage>>();
        private static object lockObj = new object();
        public static void RegisterServiceActor(IServiceActor<AmpMessage> actor)
        {
            if (!actorDict.ContainsKey(actor.Id))
            {
                lock (lockObj)
                {
                    if (!actorDict.ContainsKey(actor.Id))
                    {
                        actorDict.Add(actor.Id, actor);
                    }
                }
            }           
        }
        public static IServiceActor<AmpMessage> GetServiceActor(string actorId)
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
