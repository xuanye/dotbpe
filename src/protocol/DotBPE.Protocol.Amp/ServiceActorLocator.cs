using DotBPE.Rpc;
using DotBPE.Rpc.DefaultImpls;

namespace DotBPE.Protocol.Amp
{
    public class ServiceActorLocator: IServiceActorLocator<AmpMessage>
    {
        private static readonly HeartbeatActor<AmpMessage> HeartbeatActor =new HeartbeatActor<AmpMessage>();
        private readonly IServiceActorContainer<AmpMessage> _container;
        public ServiceActorLocator(IServiceActorContainer<AmpMessage> container){
            this._container = container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IServiceActor<AmpMessage> LocateServiceActor(AmpMessage message)
        {
            if(message.ServiceId ==0 ){ //心跳消息
               return HeartbeatActor;
            }

            //以下的是本地服务的实现
            string msgActorId = $"{message.ServiceId}${message.MessageId}";
            var msgActor = _container.GetById(msgActorId);
            if(msgActor != null)
            {
                return msgActor;
            }

            string serviceActorId = message.ServiceId + "$0";
            var serviceActor = _container.GetById(serviceActorId);
            if(serviceActor != null)
            {
                return serviceActor;
            }

            return NotFoundServiceActor.Default;
        }
    }
}
