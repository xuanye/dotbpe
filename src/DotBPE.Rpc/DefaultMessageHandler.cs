using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    /// <summary>
    /// 默认的消息处理程序
    /// </summary>
    public class DefaultMessageHandler:IMessageHandler<IMessage>
    {
        private readonly IServiceActorLocator<IMessage> _actorLocator;
        private readonly ILogger _logger;
        public DefaultMessageHandler(IServiceActorLocator<IMessage> actorLocator,ILogger logger)
        {
            this._actorLocator = actorLocator;
            this._logger = logger;
        }

        public Task RecieveAsync(IRpcContext<IMessage> context, IMessage message)
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
                    actor.Recieve(context, message);
                });               
            }
        }
    }
}
