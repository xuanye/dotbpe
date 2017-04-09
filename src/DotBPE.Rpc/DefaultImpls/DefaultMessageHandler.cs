using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    /// <summary>
    /// 默认的消息处理程序
    /// </summary>
    public class DefaultMessageHandler<TMessage>:IMessageHandler<TMessage> where TMessage :IMessage
    {
        private readonly IServiceActorLocator<TMessage> _actorLocator;
        private readonly ILogger<DefaultMessageHandler<TMessage>> _logger;
        public DefaultMessageHandler(IServiceActorLocator<TMessage> actorLocator,ILogger<DefaultMessageHandler<TMessage>> logger)
        {
            this._actorLocator = actorLocator;
            this._logger = logger;
        }

        public Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message)
        {
            var actor =  this._actorLocator.LocateServiceActor(message);
            if(actor == null) // 找不到对应的执行程序
            {                
                this._logger.LogError("消息 ${message},没有配置的处理程序");
                return Task.CompletedTask;
            }
            else
            {
                return Task.Run(() =>
                {
                    actor.Receive(context, message);
                });               
            }
        }
    }
}
