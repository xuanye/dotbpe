using DotBPE.Rpc.Codes;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IMessageHandler<TMessage> where TMessage : InvokeMessage
    {
        Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
    }
}
