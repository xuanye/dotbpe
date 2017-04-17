using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc;

namespace DotBPE.Protocol.Amp
{
    public class ServiceActorLocator: IServiceActorLocator<AmpMessage>
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IServiceActor<AmpMessage> LocateServiceActor(AmpMessage message)
        {
            if(message.ServiceId ==0 ){ //心跳消息
               return HeartbeatActor.Default;
            }

            //以下的是本地服务的实现
            string serviceActorId = message.ServiceId + "$0";
            var serviceActor = SimpleServiceActorFactory.GetServiceActor(serviceActorId);
            if(serviceActor != null)
            {
                return serviceActor;
            }
            string msgActorId = $"{message.ServiceId}${message.MessageId}";
            var msgActor = SimpleServiceActorFactory.GetServiceActor(msgActorId);
            if(msgActor != null)
            {
                return msgActor;
            }
            return  NotFoundServiceActor.Default;
        }
    }
}
