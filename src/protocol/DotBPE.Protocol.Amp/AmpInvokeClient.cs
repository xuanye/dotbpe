using System;
using System.Text;
using System.Collections.Generic;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;


namespace DotBPE.Protocol.Amp
{
    public abstract class AmpInvokeClient : InvokeClientBase<AmpMessage>
    {
        public AmpInvokeClient(IMessageSender<AmpMessage> sender):base(new AmpCallInvoker(sender))
        {
        }
    }
}
