using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public abstract class AbstractServiceActorLocator<TMessage> : IServiceActorLocator<TMessage> where TMessage :InvokeMessage
    {
        private static readonly HeartbeatActor<TMessage> HeartbeatActor = new HeartbeatActor<TMessage>();
        private readonly IServiceActorContainer<TMessage> _container;

        public AbstractServiceActorLocator(IServiceActorContainer<TMessage> container)
        {
            this._container = container;
        }

        /// <summary>
        /// Locate Service Actor
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual IServiceActor<TMessage> LocateServiceActor(TMessage message)
        {
            if (message.IsHeartBeat )
            { //心跳消息
                return HeartbeatActor;
            }

            //以下的是本地服务的实现
           
            var serviceActor = _container.GetById(message.ServiceIdentifier);
            if (serviceActor != null)
            {
                return serviceActor;
            }
                     
            var msgActor = _container.GetById(message.MethodIdentifier);
            if (msgActor != null)
            {
                return msgActor;
            }

            return LocateDefaultServiceActor(message);
        }

        protected abstract IServiceActor<TMessage> LocateDefaultServiceActor(TMessage message);
    }
}
