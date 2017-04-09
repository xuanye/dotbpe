using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IClientBootstrap<TMessage> where TMessage :IMessage
    {
        Task<IRpcContext<TMessage>> ConnectAsync(EndPoint endpoint);

        event EventHandler<EndPoint> Disconnected;
    }
}
