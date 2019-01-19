using DotBPE.Baseline.Utility;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Options;
using Peach;
using Peach.Config;
using Peach.Protocol;
using Peach.Tcp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultTransport : ITransport<AmpMessage>
    {
        private readonly ISocketContext<AmpMessage> _context;
        public DefaultTransport(ISocketContext<AmpMessage> context)
        {
            this._context = context;
            Id = ObjectId.GenerateNewId().ToString();
        }

        public string Id { get; }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return this._context.Channel.CloseAsync();
        }

        public Task SendAsync(AmpMessage request)
        {
            return this._context.SendAsync(request);
        }
    }
}
