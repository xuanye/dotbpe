using Peach.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface IRpcClient<in TMessage> where TMessage : IMessage
    {

        Task SendAsync(TMessage message);
        Task CloseAsync(CancellationToken cancellationToken);
    }
}
