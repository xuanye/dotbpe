// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Rpc.Server;
using Peach;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    public class PeachServerHost : IServerHost
    {
        private readonly IServerBootstrap _server;

        public PeachServerHost(IServerBootstrap server)
        {
            _server = server;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync(cancellationToken);
        }
    }
}
