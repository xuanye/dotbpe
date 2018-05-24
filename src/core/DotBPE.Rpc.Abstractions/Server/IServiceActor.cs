using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServiceActor<TMessage> where TMessage : InvokeMessage
    {
        string Id { get; }

        Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message);
    }
}
