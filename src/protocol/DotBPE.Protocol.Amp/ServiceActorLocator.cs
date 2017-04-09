using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc;

namespace DotBPE.Protocol.Amp
{
    public class ServiceActorLocator: IServiceActorLocator<AmpMessage>
    {
        public IServiceActor<AmpMessage> LocateServiceActor(AmpMessage message)
        {
            Console.WriteLine($"收到请求消息,服务号{message.ServiceId},消息号:{message.MessageId}");
            return new EchoServiceActor();
        }
    }
}
