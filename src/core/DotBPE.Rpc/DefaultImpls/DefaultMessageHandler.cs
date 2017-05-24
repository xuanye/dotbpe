using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    /// <summary>
    /// 默认的消息处理程序
    /// </summary>
    public class DefaultMessageHandler<TMessage>:IMessageHandler<TMessage> where TMessage :InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultMessageHandler<TMessage>>();

        private readonly IServiceActorLocator<TMessage> _actorLocator;



        public DefaultMessageHandler(IServiceActorLocator<TMessage> actorLocator)
        {
            this._actorLocator = actorLocator;
        }

        /// <summary>
        /// 消息处理订阅，可以添加日志等等功能
        /// </summary>
        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;


        private void RaiseReceivedEvent(IRpcContext<TMessage> context, TMessage message)
        {
            Recieved?.Invoke(this, new MessageRecievedEventArgs<TMessage>(context, message));
        }
        public virtual Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message)
        {
            RaiseReceivedEvent(context, message);

            //本地服务互调也会进入到这里，但是这里不能再重复处理了，不然就是死循环
            if(message.InvokeMessageType != InvokeMessageType.Request)
            {
                return Utils.TaskUtils.CompletedTask;
            }

            var actor =  this._actorLocator.LocateServiceActor(message);
            if(actor == null) // 找不到对应的执行程序
            {
                Logger.Error("IServiceActor NOT FOUND");
                return Utils.TaskUtils.CompletedTask;
            }
            else
            {
                return actor.ReceiveAsync(context, message);


            }
        }
    }
}
