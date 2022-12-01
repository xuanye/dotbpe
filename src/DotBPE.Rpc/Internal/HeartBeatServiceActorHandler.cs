// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Internal
{
    public class HeartBeatServiceActorHandler : IServiceActorHandler
    {

        public static HeartBeatServiceActorHandler Instance = new HeartBeatServiceActorHandler();

        public Task HandleAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            return HeartBeatServiceActor.Instance.ReceiveAsync(context, message);
        }
    }
}
