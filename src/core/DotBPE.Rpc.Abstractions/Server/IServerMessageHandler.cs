using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServerMessageHandler<TMessage> where TMessage : InvokeMessage
    {
        Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message);
    }
}
