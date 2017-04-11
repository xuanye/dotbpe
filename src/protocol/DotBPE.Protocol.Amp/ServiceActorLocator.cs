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
            //TODO: 
            //step1: 根据服务号消息号查找配置，看是否在配置文件中配置了，远端服务号和消息号
            //step2: 如果是远端服务，则获取默认远端服务调用者RemoteServiceActor
            //step3: 否则通过服务注册器 获取本地服务实现。
            
            
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
            return new NotFoundServiceActor();
        }
    }
}
