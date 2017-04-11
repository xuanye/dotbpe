using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protocol.Amp
{
    public abstract class AmpInvokeClient : InvokeClientBase<AmpMessage>
    {
        public AmpInvokeClient(IRpcClient<AmpMessage> rpcClient):base(new AmpCallInvoker(rpcClient))
        {           
        }
    }
}
