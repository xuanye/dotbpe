using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultTransport<TMessage> : ITransport<TMessage>, IDisposable
    where TMessage : InvokeMessage
    {
        private readonly IRpcContext<TMessage> _context;
        private readonly ILogger Logger;

        public DefaultTransport(IRpcContext<TMessage> context, ILoggerFactory factory)
        {
            this._context = context;
            this.Id = IdUtils.NewId();
            this.Logger = factory.CreateLogger(this.GetType());
        }

        public async Task CloseAsync()
        {
            await this._context.CloseAsync();
            this.Dispose();
        }

        public string Id { get; private set; }

        public void Dispose()
        {
        }

        public Task SendAsync(TMessage message)
        {
            return this._context.SendAsync(message);
        }
    }
}
