using DotBPE.Rpc.Codes;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    internal class LocalMockContext<TMessage> : IRpcContext<TMessage> where TMessage : InvokeMessage
    {
        private readonly IClientMessageHandler<TMessage> _handler;

        public LocalMockContext(IClientMessageHandler<TMessage> handler)
        {
            this._handler = handler;
        }

        public EndPoint RemoteAddress => null;

        public EndPoint LocalAddress => null;

        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendAsync(TMessage data)
        {
            this._handler.Receive(this, data);
            return Utils.TaskUtils.CompletedTask;
        }
    }
}
