using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultTransport<TMessage> : ITransport<TMessage>, IDisposable 
        where TMessage : InvokeMessage
    {
        private readonly IRpcContext<TMessage> _context;
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultTransport<TMessage>>();


        public DefaultTransport(IRpcContext<TMessage> context) 
        {
            this._context = context;          
        }

        public async Task CloseAsync()
        {         
            await this._context.CloseAsync();
            this.Dispose();
        }

        public void Dispose()
        {
            //TODO:清理缓存
        }

        public async Task SendAsync(TMessage message)
        {
            try
            {
                Logger.Debug("准备发送消息。");
              
                try
                {
                    //发送
                    await this._context.SendAsync(message);
                }
                catch (Exception exception)
                {
                    throw new RpcCommunicationException("与服务端通讯时发生了异常。", exception);
                }
                Logger.Debug("消息发送成功。");
            }
            catch (Exception exception)
            {
                Logger.Error("消息发送失败。", exception);
                throw;
            }
        }
    }
}
