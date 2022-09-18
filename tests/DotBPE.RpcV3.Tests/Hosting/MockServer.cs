// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.RpcV3.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Tests.Hosting
{
    public class MockServer : IServerHost
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
