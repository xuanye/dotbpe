// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peach;
using Peach.Tcp;

namespace DotBPE.Rpc.Hosting
{
    public class PeachServerBootstrap : TcpServerBootstrap<AmpMessage>
    {
        public PeachServerBootstrap(
           ISocketService<AmpMessage> socketService,
           IChannelHandlerPipeline handlerPipeline,
           ILoggerFactory loggerFactory,
           IOptions<RpcServerOptions> hostOption)
           : base(socketService, handlerPipeline, loggerFactory, hostOption)
        {
        }
    }
}
