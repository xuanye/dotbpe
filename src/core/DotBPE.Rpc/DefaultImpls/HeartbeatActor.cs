using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc.DefaultImpls
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