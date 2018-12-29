using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ITransportFactory<TMessage>  where TMessage : IMessage
    {
        Task<ITransport<TMessage>> CreateTransport(EndPoint endpoint);

        Task CloseTransportAsync(EndPoint endpoint);

        Task CloseAllTransports(CancellationToken cancellationToken);
    }
}
