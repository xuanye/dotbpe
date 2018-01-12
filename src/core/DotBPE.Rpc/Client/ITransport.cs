using DotBPE.Rpc.Codes;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface ITransport<in TMessage> where TMessage : InvokeMessage
    {
        Task SendAsync(TMessage request);

        Task CloseAsync();

        string Id { get; }
    }
}
