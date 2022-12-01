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
    public class HeartBeatServiceActor : IServiceActor<AmpMessage>
    {
        public static HeartBeatServiceActor Instance = new HeartBeatServiceActor();
        public string Id => "0.0";

        public string GroupName { get; } = "default";


        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            message.MessageType = RpcMessageType.Response;
            return context.SendAsync(message);
        }
    }
}
