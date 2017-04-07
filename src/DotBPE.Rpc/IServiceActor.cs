using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IServiceActor<TMessage> where TMessage :IMessage
    {
        Task Recieve(IRpcContext<TMessage> context, TMessage message);
    }
}
