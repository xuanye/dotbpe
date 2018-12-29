using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ICallInvoker<TMessage> where TMessage:IMessage
    {
        TMessage BlockingCall(TMessage request, int timeOut = 3000);

        Task<TMessage> AsyncCall(TMessage request, int timeOut = 3000);
    }
}
