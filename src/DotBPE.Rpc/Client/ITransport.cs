using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ITransport<TMessage> where TMessage : IMessage
    {
        Task SendAsync(TMessage request);


        Task CloseAsync(CancellationToken cancellationToken);

        string Id { get; }
    }
}
