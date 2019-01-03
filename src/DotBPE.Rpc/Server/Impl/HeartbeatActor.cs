using DotBPE.Rpc.Protocol;
using Peach;
using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class HeartbeatActor : IServiceActor<AmpMessage>
    {
        public string Id => "0$0";



        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            message.InvokeMessageType = InvokeMessageType.Response;
            return context.SendAsync(message);
        }
    }
}
