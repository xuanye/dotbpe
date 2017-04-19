using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class HeartbeatActor : IServiceActor<AmpMessage>
    {
        public static HeartbeatActor Default = new HeartbeatActor();
        public string Id => "Heartbeat";

        public Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            message.InvokeMessageType = InvokeMessageType.Response;
            return context.SendAsync(message);
        }
    }
}