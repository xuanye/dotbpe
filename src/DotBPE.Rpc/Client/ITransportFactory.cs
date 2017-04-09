using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface ITransportFactory<in TMessage> where TMessage : InvokeMessage
    {
        ITransport<TMessage> CreateTransport(EndPoint endpoint);
    }
}
