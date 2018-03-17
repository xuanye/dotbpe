using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {
    public interface IClientMessageHandler<TMessage> where TMessage : InvokeMessage {
        void Receive (IRpcContext<TMessage> context, TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
    }
}