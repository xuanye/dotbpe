using DotBPE.Rpc.Codes;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public class LocalMockContext<TMessage> : IRpcContext<TMessage> where TMessage : InvokeMessage
    {
        private readonly IMessageHandler<TMessage> _handler;

        public LocalMockContext(IMessageHandler<TMessage> handler)
        {
            this._handler = handler;
        }

        public string RemoteAddress => "127.0.0.1";

        public string LocalAddress => "127.0.0.1";

        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendAsync(TMessage data)
        {
            return this._handler.ReceiveAsync(this, data);
        }
    }
}
