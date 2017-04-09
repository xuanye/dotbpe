using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IRpcClient<in TMessage> where TMessage : InvokeMessage
    {
        Task SendAsync(EndPoint serverAddrss, TMessage message);
        Task SendAsync(TMessage message);
    }
}
