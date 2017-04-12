using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc;

namespace DotBPE.Protocol.Amp
{
    public class ProxyMessageSender : IMessageSender<AmpMessage>
    {
        private readonly IServiceActorLocator<AmpMessage> _ActorLocator;
        private readonly ITransportFactory<AmpMessage> _Factory;
        private readonly IMessageHandler<AmpMessage> _Handler;

        public ProxyMessageSender(IServiceActorLocator<AmpMessage> actorLocator,
        ITransportFactory<AmpMessage> factory,
        IMessageHandler<AmpMessage>  handler){
            this._ActorLocator = actorLocator;
            this._Factory = factory;
            this._Handler = handler;
            this._Handler.Recieved += Message_Received;
        }

        private void Message_Received(object sender, MessageRecievedEventArgs<AmpMessage> e)
        {
           //作为客户端的处理
           //TODO:我发的我来处理，不是我发的我不处理
           Recieved?.Invoke(this,e);
        }

        public event EventHandler<MessageRecievedEventArgs<AmpMessage>> Recieved;

        public Task SendAsync(AmpMessage message)
        {
            //读取配置 获取当前Message是否调用本地还是调用远端服务
            var transport = this._Factory.CreateTransport(new IPEndPoint(IPAddress.Parse("127.0.0.1"),6201) );

            transport.SendAsync(message);

            var actor = this._ActorLocator.LocateServiceActor(message);
            if(actor !=null){
               return actor.Receive(null,message);
            }
            return Task.CompletedTask;
        }
    }
}