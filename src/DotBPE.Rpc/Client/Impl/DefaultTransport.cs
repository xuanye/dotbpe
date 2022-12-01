// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Utility;
using DotBPE.Rpc.Protocols;
using Peach;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    internal class DefaultTransport : ITransport
    {
        private readonly ISocketContext<AmpMessage> _context;
        public DefaultTransport(ISocketContext<AmpMessage> context)
        {
            _context = context;
            Id = ObjectId.GenerateNewId().ToString();
        }

        public string Id { get; }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return _context.Channel.CloseAsync();
        }

        public Task SendAsync(AmpMessage request)
        {
            return _context.SendAsync(request);
        }
    }
}
