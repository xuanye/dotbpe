using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;

namespace PiggyMetrics.Common
{
    public abstract class ServiceActorBase : IServiceActor<AmpMessage>
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ServiceActorBase>();
        public abstract string Id {get;}

        public abstract Task ReceiveAsync(IRpcContext<AmpMessage> context, AmpMessage message);

        protected  Task ReceiveNotFoundAsync(IRpcContext<AmpMessage> context, AmpMessage req)
        {
            var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            response.Sequence = req.Sequence;
            response.InvokeMessageType = DotBPE.Rpc.Codes.InvokeMessageType.NotFound;
            Logger.Error("recieve message serviceId={0},messageId={1},Length ={2},Actor NotFound",req.ServiceId,req.MessageId,req.Length);
            return context.SendAsync(response);
        }

    }
}
