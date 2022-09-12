// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Abstractions
{
    public interface IServiceActorHandler
    {
        Task HandleAsync(ISocketContext<AmpMessage> context, AmpMessage message);
    }
}
