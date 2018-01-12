using DotBPE.Rpc.Codes;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServiceActor<TMessage> where TMessage : IMessage
    {
        string Id { get; }

        Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message);
    }
}
