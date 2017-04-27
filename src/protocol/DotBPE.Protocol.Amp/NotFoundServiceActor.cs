using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class NotFoundServiceActor: IServiceActor<AmpMessage>
    {
        public static NotFoundServiceActor Default  = new NotFoundServiceActor();

        public string Id
        {
            get { return "NotFoundServiceActor"; }
        }

        public async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            AmpMessage response = new AmpMessage
            {
                InvokeMessageType = InvokeMessageType.NotFound,
                ServiceId = message.ServiceId,
                MessageId = message.MessageId
            };
            response.Sequence = message.Sequence;
            await context.SendAsync(response);
        }
    }
}
