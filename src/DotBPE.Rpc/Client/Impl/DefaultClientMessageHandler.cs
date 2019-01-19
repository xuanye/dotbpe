using DotBPE.Rpc.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public class DefaultClientMessageHandler : IClientMessageHandler<AmpMessage>
    {
        public event EventHandler<AmpMessage> OnReceived;

        public void RaiseReceive(AmpMessage message)
        {
            OnReceived?.Invoke(this, message);
        }
    }
}
