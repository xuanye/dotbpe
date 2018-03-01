using DotBPE.Rpc.Codes;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class HeartbeatActor<TMessage> : IServiceActor<TMessage> where TMessage : InvokeMessage
    {
        public string Id => "heartbeat";

        public Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message)
        {
            message.InvokeMessageType = InvokeMessageType.Response;
            return context.SendAsync(message);
        }
    }
}
