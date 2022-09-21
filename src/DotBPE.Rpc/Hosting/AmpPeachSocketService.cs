// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    public class AmpPeachSocketService : AbsSocketService<AmpMessage>
    {
        private readonly IMessageHandler<AmpMessage> _messageHandler;
        private readonly ILogger<AmpPeachSocketService> _logger;
        public AmpPeachSocketService(
            IMessageHandler<AmpMessage> messageHandler,
            ILogger<AmpPeachSocketService> logger
            )
        {
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public override void OnReceive(ISocketContext<AmpMessage> context, AmpMessage msg)
        {
            _logger.LogDebug("receive message {id}", msg.Id);
            Task.Run(async () =>
            {
                await _messageHandler.ReceiveAsync(context, msg);
            }).ConfigureAwait(false);
        }

        public override void OnException(ISocketContext<AmpMessage> context, Exception ex)
        {
            _logger.LogError(ex, "server error occ");
            base.OnException(context, ex);
        }

        public override void OnDisconnected(ISocketContext<AmpMessage> context)
        {
            _logger.LogInformation("client disconnected from {address}", context.RemoteEndPoint.Address);
            base.OnDisconnected(context);
        }

        public override void OnConnected(ISocketContext<AmpMessage> context)
        {
            _logger.LogInformation("client connected from {address}", context.RemoteEndPoint.Address);
            base.OnConnected(context);
        }
    }
}
