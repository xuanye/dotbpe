// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using Peach;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
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
            _logger.LogDebug("Received a message from {RemoteAddress},messageId={MessageId}", context.RemoteEndPoint, message.Id);
            if (message.MessageType == RpcMessageType.Response)
                return Task.CompletedTask;

            if (message.IsHeartBeat)
            {
                return HeartBeatServiceActorHandler.Instance.HandleAsync(context, message);
            }

            var handler = _actorHandlerFactory.GetInstance(message.MethodIdentifier);
            if (handler != null)
                return handler.HandleAsync(context, message);

            _logger.LogWarning("The service actor hander is not found,MethodId={methodIdentifier}", message.MethodIdentifier);
            return NotFoundServiceActorHandler.Instance.HandleAsync(context, message);
        }
    }
}
