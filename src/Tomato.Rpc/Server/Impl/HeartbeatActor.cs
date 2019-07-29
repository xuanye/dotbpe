using Tomato.Rpc.Protocol;
using Peach;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Rpc.Server
{
    public class HeartbeatActor : IServiceActor<AmpMessage>
    {
        public string Id => "0.0";

        public string GroupName { get; } = "default";


        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            message.InvokeMessageType = InvokeMessageType.Response;
            return context.SendAsync(message);
        }
    }
}
