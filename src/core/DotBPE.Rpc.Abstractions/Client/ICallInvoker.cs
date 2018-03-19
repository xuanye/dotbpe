using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {
    public interface ICallInvoker<TMessage> : IDisposable where TMessage : InvokeMessage {
        IRpcClient<TMessage> RpcClient { get; }

        TMessage BlockingCall (TMessage request, int timeOut = 3000);

        Task<TMessage> AsyncCall (TMessage request, int timeOut = 3000);
    }
}