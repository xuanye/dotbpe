using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ICallInvoker<TMessage> where TMessage:IMessage
    {

        Task<TMessage> AsyncCall(TMessage request, int timeout = 3000);

        Task<RpcResult<T2>> AsyncCall<T1,T2>(string callName,T1 reqObj,ushort serviceId,ushort messageId,int timeout=3000);

        Task<RpcResult> AsyncCallWithoutResponse<T1>(string callName,T1 reqObj,ushort serviceId,ushort messageId);

    }
}
