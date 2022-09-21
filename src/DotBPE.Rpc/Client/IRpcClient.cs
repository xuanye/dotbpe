// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface IRpcClient
    {

        Task SendAsync(AmpMessage message);
        Task CloseAsync(CancellationToken cancellationToken);

    }
}
