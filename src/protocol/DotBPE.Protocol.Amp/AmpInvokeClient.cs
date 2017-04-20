using DotBPE.Rpc;
using DotBPE.Rpc.Client;


namespace DotBPE.Protocol.Amp
{
    public abstract class AmpInvokeClient : InvokeClientBase<AmpMessage>
    {
        public AmpInvokeClient(IRpcClient<AmpMessage> client):base(new AmpCallInvoker(client))
        {
        }
    }
}
