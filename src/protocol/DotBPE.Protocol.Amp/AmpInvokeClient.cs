using DotBPE.Rpc;
using DotBPE.Rpc.Client;

namespace DotBPE.Protocol.Amp
{
    public class AmpInvokeClient : InvokeClientBase<AmpMessage>
    {
        public AmpInvokeClient(ICallInvoker<AmpMessage> callInvoker) : base(callInvoker)
        {

        }
    }
}
