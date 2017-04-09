using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
   
    public interface IRpcContext<in TMessage> where TMessage:IMessage
    {
        Task SendAsync(TMessage data);
    }
}
