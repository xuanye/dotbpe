using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    /// <summary>
    /// 默认的消息处理程序
    /// </summary>
    public class ServerMessageHandler<TMessage> : IServerMessageHandler<TMessage> where TMessage : InvokeMessage
    {
        private readonly ILogger<ServerMessageHandler<TMessage>> Logger;

        private readonly IServiceActorLocator<TMessage> _actorLocator;

        public ServerMessageHandler(IServiceActorLocator<TMessage> actorLocator, ILogger<ServerMessageHandler<TMessage>> logger)
        {
            this._actorLocator = actorLocator;
            this.Logger = logger;
        }

        public virtual Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message)
        {
            if (message.InvokeMessageType != InvokeMessageType.Request)
            {
                return Utils.TaskUtils.CompletedTask;
            }

            var actor = this._actorLocator.LocateServiceActor(message);
            if (actor == null) // 找不到对应的执行程序
            {
                Logger.LogError("IServiceActor NOT FOUND,MethodId={methodIdentifier}", message.MethodIdentifier);
                return Utils.TaskUtils.CompletedTask;
            }
            else
            {
                return actor.ReceiveAsync(context, message);
            }
        }
    }
}
