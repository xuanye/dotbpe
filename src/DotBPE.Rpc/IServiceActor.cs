using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public interface IServiceActor<TMessage> where TMessage :IMessage
    {
        string Id { get; }
        Task Receive(IRpcContext<TMessage> context, TMessage message);
    }
}
