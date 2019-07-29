using Tomato.Baseline.Utility;
using Tomato.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Peach;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Rpc.Client
{
    public class InprocContext : ISocketContext<AmpMessage>
    {
        private readonly IClientMessageHandler<AmpMessage> _handler;
        public InprocContext(IClientMessageHandler<AmpMessage> handler)
        {
            this._handler = handler;
            Id = ObjectId.GenerateNewId().ToString();
        }
        public bool Active { get { return true; } }
        public IChannel Channel { get; }
        public string Id { get; set; }

        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }

        public Task SendAsync(AmpMessage message)
        {
            this._handler.RaiseReceive(message);
            return Task.CompletedTask;
        }
    }
}
