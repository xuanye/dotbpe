using DotBPE.Baseline.Utility;
using DotBPE.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Peach;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class InprocContext : ISocketContext<AmpMessage>
    {
        private readonly IClientMessageHandler<AmpMessage> _handler;
        public InprocContext(IClientMessageHandler<AmpMessage> handler)
        {
            _handler = handler;
            Id = ObjectId.GenerateNewId().ToString();
        }
        public bool Active { get { return true; } }
        public IChannel Channel { get; }
        public string Id { get; set; }

        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }

        public Task SendAsync(AmpMessage message)
        {
            _handler.RaiseReceive(message);
            return Task.CompletedTask;
        }
    }
}
