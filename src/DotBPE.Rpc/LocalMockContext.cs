using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc
{
    public class LocalMockContext<TMessage> : IRpcContext<TMessage> where TMessage : InvokeMessage
    {
        private readonly IMessageHandler<TMessage> _handler;
        public LocalMockContext(IMessageHandler<TMessage> handler){
            this._handler = handler;
        }
        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendAsync(TMessage data)
        {
            return this._handler.ReceiveAsync(this,data);
        }
    }
}