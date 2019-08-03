using Peach;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Rpc.Server
{
    public interface IServiceActor<TMessage> :IRpcService where TMessage : IMessage
    {
        string Id { get; }
        string GroupName { get; }

        Task ReceiveAsync(ISocketContext<TMessage> context, TMessage message);
    }
}
