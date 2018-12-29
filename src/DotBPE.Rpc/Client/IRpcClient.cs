using Peach.EventArgs;
using Peach.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface IRpcClient<TMessage> where TMessage : IMessage
    {
      
        Task SendAsync(TMessage message);
        Task CloseAsync(CancellationToken cancellationToken);             
    }
}
