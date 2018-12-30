using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class DefaultServerMessageHandler : IServerMessageHandler<AmpMessage>
    {
        private readonly IServiceActorLocator<AmpMessage> _actorLocator;
        private readonly ILogger<DefaultServerMessageHandler> _logger;
        public DefaultServerMessageHandler(
            IServiceActorLocator<AmpMessage> actorLocator,
            ILogger<DefaultServerMessageHandler> logger
            )
        {
            _actorLocator = actorLocator;
            _logger = logger;
        }

        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            if (message.InvokeMessageType != InvokeMessageType.Request)
            {
                return Task.CompletedTask;
            }

            var actor = this._actorLocator.LocateServiceActor(message.MethodIdentifier);
            if (actor == null) // 找不到对应的执行程序
            {
                _logger.LogError("the service actor is not found,MethodId={methodIdentifier}", message.MethodIdentifier);
                return NotFoundServiceActor.Default.ReceiveAsync(context,message);
            }

            return actor.ReceiveAsync(context, message);

        }
    }
}
