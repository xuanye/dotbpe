using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc.Server {
    public class NoopServiceActorLocator<TMessage> : IServiceActorLocator<TMessage> where TMessage : InvokeMessage {
        public IServiceActor<TMessage> LocateServiceActor (TMessage message) {
            return null;
        }
    }
}