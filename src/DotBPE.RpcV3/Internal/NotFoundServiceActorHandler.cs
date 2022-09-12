// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Protocols;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Internal
{
    public class NotFoundServiceActorHandler : IServiceActorHandler
    {
        public static NotFoundServiceActorHandler Instance = new NotFoundServiceActorHandler();

        public Task HandleAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            return NotFoundServiceActor.Instance.ReceiveAsync(context, message);
        }
    }
}
