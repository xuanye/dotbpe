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

        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

        public Task ReceiveAsync(IRpcContext<TMessage> context, TMessage message)
        {
            var actor =  this._actorLocator.LocateServiceActor(message);
            if(actor == null) // 找不到对应的执行程序
            {
                Logger.Error("消息 ${message},没有配置的处理程序");
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
