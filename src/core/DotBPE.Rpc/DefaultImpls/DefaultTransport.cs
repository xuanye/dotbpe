using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.DefaultImpls
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

        public async Task SendAsync(TMessage message)
        {
            try
            {
                try
                {
                    //发送
                    await this._context.SendAsync(message);
                }
                catch (Exception exception)
                {
                    throw new RpcCommunicationException("send message error", exception);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("send message error", exception);
                throw;
            }
        }
    }
}
