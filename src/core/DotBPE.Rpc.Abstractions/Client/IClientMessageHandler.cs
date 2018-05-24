using System;

namespace DotBPE.Rpc
{
    public interface IClientMessageHandler<TMessage> where TMessage : InvokeMessage
    {
        void Receive(IRpcContext<TMessage> context, TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
    }
}
