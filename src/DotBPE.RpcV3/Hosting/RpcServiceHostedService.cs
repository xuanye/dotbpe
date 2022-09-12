// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    internal class RpcServiceHostedService : IHostedService
    {
        private readonly IServerHost _server;

        public RpcServiceHostedService(IServerHost server)
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
