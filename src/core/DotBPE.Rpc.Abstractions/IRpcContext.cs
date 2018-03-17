using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {

    public interface IRpcContext {
        EndPoint RemoteAddress { get; }
        EndPoint LocalAddress { get; }
    }

    public interface IRpcContext<in TMessage> : IRpcContext where TMessage : InvokeMessage {
        Task SendAsync (TMessage msg);
        Task CloseAsync ();
    }
}