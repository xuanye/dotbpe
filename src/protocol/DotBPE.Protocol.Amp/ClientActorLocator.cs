using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class ClientActorLocator: IServiceActorLocator<AmpMessage>
    {

        private readonly IServiceActor<AmpMessage> _actor;
        public ClientActorLocator(IServiceActor<AmpMessage> actor)
        {
            _actor = actor;
        }
        public IServiceActor<AmpMessage> LocateServiceActor(AmpMessage message)
        {
            //这里可以做一些特殊的处理，比如过滤一些特定的消息
            return _actor;
        }
    }

    public class DefaultClientActor : IServiceActor<AmpMessage>
    {
        public string Id => "defaultActor";

        public Task Receive(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            //一些特殊的处理
            if (Recieved != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    Recieved(this, new MessageRecievedEventArgs<AmpMessage>(context, message));
                });
            }
            return Task.CompletedTask;

        }

        public event EventHandler<MessageRecievedEventArgs<AmpMessage>> Recieved;
    }

    public class MessageRecievedEventArgs<TMessage> where TMessage:InvokeMessage
    {
        public MessageRecievedEventArgs()
        {
            
        }
        public MessageRecievedEventArgs(IRpcContext<TMessage> context, TMessage message)
        {

            this.Context = context;
            this.Message = message;
        }

        public IRpcContext<TMessage> Context{ get; set; }
        public TMessage Message { get; set; }

    }
}
