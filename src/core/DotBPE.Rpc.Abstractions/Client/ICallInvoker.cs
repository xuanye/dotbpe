using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface ICallInvoker<TMessage> : IDisposable where TMessage : InvokeMessage
    {
        IRpcClient<TMessage> RpcClient { get; }

        TMessage BlockingCall(TMessage request, int timeOut = 3000);

        Task<TMessage> AsyncCall(TMessage request, int timeOut = 3000);
    }
}
