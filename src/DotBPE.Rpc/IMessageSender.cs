using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IMessageSender<TMessage> where TMessage :InvokeMessage
    {
        Task SendAsync(TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
    }
}