using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IClientMessageHandler<TMessage> where TMessage : IMessage
    {
        event EventHandler<TMessage> OnReceived;

        void RaiseReceive(TMessage message);
    }
}
