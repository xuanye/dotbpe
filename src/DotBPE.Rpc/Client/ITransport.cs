// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface ITransport
    {
        Task SendAsync(AmpMessage request);

        Task CloseAsync(CancellationToken cancellationToken);

        string Id { get; }
    }
}
