using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IRpcContext<TMessage> where TMessage:IMessage
    {
        Task SendAsync(TMessage data);
        Task SendAndFlushAsync(TMessage data);
    }
}
