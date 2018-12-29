using DotBPE.Rpc.Protocol;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class NotFoundServiceActor : IServiceActor<AmpMessage>
    {
        public static NotFoundServiceActor Default = new NotFoundServiceActor();

        public string Id => "NotFoundServiceActor";       
        public string Category => "Default";

        public async Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            AmpMessage response = new AmpMessage
            {
                InvokeMessageType = InvokeMessageType.Response,
                Code = RpcErrorCodes.CODE_SERVICE_NOT_FOUND,
                ServiceId = message.ServiceId,
                MessageId = message.MessageId
            };
            response.Sequence = message.Sequence;
            await context.SendAsync(response);
        }
    }
}
