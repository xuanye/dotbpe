using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultTransport<TMessage> : ITransport<TMessage>, IDisposable 
        where TMessage : InvokeMessage
    {
        private readonly IRpcContext<TMessage> _context;
        private readonly ILogger _logger;
     

       

        public DefaultTransport(IRpcContext<TMessage> context, ILogger logger) 
        {
            this._context = context;
            this._logger = logger;
        }
       
        
        
        public void Dispose()
        {
            //TODO:清理缓存
        }

        public async Task SendAsync(TMessage message)
        {
            try
            {
                _logger.LogDebug("准备发送消息。");
              
                try
                {
                    //发送
                    await this._context.SendAsync(message);
                }
                catch (Exception exception)
                {
                    throw new RpcCommunicationException("与服务端通讯时发生了异常。", exception);
                }
                 _logger.LogDebug("消息发送成功。");
            }
            catch (Exception exception)
            {
                _logger.LogError("消息发送失败。", exception);
                throw;
            }
        }
    }
}
