using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IMessageHandler<TMessage> where TMessage : InvokeMessage
    {
        Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

    }
}
