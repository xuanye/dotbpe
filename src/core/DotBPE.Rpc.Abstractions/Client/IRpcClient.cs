using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IRpcClient<TMessage> : IDisposable where TMessage : InvokeMessage
    {
        Task SendAsync(EndPoint serverAddrss, TMessage message);

        Task CloseAsync(EndPoint serverAddress);

        Task SendAsync(TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
    }
}
