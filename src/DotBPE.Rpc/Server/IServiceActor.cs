using Peach;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public interface IServiceActor<TMessage> :IRpcService where TMessage : IMessage
    {
        string Id { get; }

        Task ReceiveAsync(ISocketContext<TMessage> context, TMessage message);
    }
}
