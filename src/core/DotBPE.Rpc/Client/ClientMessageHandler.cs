using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc.Client {
    public class ClientMessageHandler<TMessage> : IClientMessageHandler<TMessage> where TMessage : InvokeMessage {
        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

        public void Receive (IRpcContext<TMessage> context, TMessage message) {
            Recieved?.Invoke (this, new MessageRecievedEventArgs<TMessage> (context, message));
        }
    }
}