using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class NotFoundServiceActor: IServiceActor<AmpMessage>
    {
        public string Id
        {
            get { return "40400$404"; }
        }

        public async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            AmpMessage response = new AmpMessage
            {
                InvokeMessageType = InvokeMessageType.Response,
                Data = Encoding.UTF8.GetBytes("服务不存在"),
                ServiceId = 40400,
                MessageId = 404
            };

            await context.SendAsync(response);
        }
    }
}
