// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ITransportFactory
    {
        Task<ITransport> CreateTransport(EndPoint endpoint);

        Task CloseTransportAsync(EndPoint endpoint);

        Task CloseAllTransports(CancellationToken cancellationToken);
    }
}
