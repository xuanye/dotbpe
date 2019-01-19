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
            this._actorLocator = actorLocator;
            this._logger = logger;
        }

        public Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            if (message.InvokeMessageType == InvokeMessageType.Response)
            {
                return Task.CompletedTask;
            }

            var actor = this._actorLocator.LocateServiceActor(message.MethodIdentifier);
            if (actor != null)
                return actor.ReceiveAsync(context, message);


            this._logger.LogError("the service actor is not found,MethodId={methodIdentifier}", message.MethodIdentifier);
            return NotFoundServiceActor.Default.ReceiveAsync(context,message);

        }
    }
}
