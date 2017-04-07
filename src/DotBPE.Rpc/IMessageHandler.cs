using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public interface IMessageHandler<TMessage> where TMessage : IMessage
    {
        Task RecieveAsync(IRpcContext<TMessage> context, TMessage message);
    }
}
