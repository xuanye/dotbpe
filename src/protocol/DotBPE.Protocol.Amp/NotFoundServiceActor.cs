using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public class NotFoundServiceActor : IServiceActor<AmpMessage>
    {
        public static NotFoundServiceActor Default = new NotFoundServiceActor();

        public string Id
        {
            get { return "NotFoundServiceActor"; }
        }

        public async Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            AmpMessage response = new AmpMessage
            {
                InvokeMessageType = InvokeMessageType.Response,
                Code = ErrorCodes.CODE_SERVICE_NOT_FOUND,
                ServiceId = message.ServiceId,
                MessageId = message.MessageId
            };
            response.Sequence = message.Sequence;
            await context.SendAsync(response);
        }
    }
}
