// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocols;
using DotBPE.RpcV3.Abstractions;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Core
{
    public class DefaultMessageHandler : IMessageHandler<AmpMessage>
    {
        private readonly IServiceActorHandlerFactory _actorHandlerFactory;
        private readonly ILogger<DefaultMessageHandler> _logger;

        public DefaultMessageHandler(IServiceActorHandlerFactory actorHandlerFactory, ILogger<DefaultMessageHandler> logger)
        {
            _actorHandlerFactory = actorHandlerFactory;
            _logger = logger;
        }
        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            _logger.LogDebug("Received a message from {remoteAddress},messageId={messageId}", context.RemoteEndPoint, message.Id);
            if (message.MessageType == RpcMessageType.Response)
            {
                return Task.CompletedTask;
            }

            var handler = _actorHandlerFactory.GetInstance(message.MethodIdentifier);
            if (handler != null)
            {
                return handler.HandleAsync(context, message);
            }

            _logger.LogError("The service actor hander is not found,MethodId={methodIdentifier}", message.MethodIdentifier);
            return NotFoundServiceActorHandler.Instance.HandleAsync(context, message);
        }
    }
}
