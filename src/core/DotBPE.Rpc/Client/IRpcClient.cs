using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IRpcClient<TMessage> where TMessage : InvokeMessage
    {
        Task SendAsync(EndPoint serverAddrss, TMessage message);

        Task CloseAsync(EndPoint serverAddress);
        Task CloseAsync();

        Task SendAsync(TMessage message);

        event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

    }
}
