using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ICallInvoker
    { 

        Task<RpcResult> AsyncCallWithOutResponse<T>(string callName,ushort serviceId,ushort messageId,T req);

        Task<RpcResult<TResult>> AsyncCall<T,TResult>(string callName, ushort serviceId, ushort messageId,T req, int timeOut = 3000) ;
    }
}
