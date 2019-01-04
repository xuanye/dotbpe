using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class AmpRpcService : AbsSocketService<AmpMessage>
    {
        private readonly IServerMessageHandler<AmpMessage> _messageHandler;
        private readonly ILogger<AmpRpcService> _logger;
        public AmpRpcService(
            IServerMessageHandler<AmpMessage> messageHandler,
            ILogger<AmpRpcService> logger
            )
        {
            this._messageHandler = messageHandler;
            this._logger = logger;
        }

        public override void OnReceive(ISocketContext<AmpMessage> context, AmpMessage msg)
        {
            this._logger.LogDebug("receive message {id}", msg.Id);
            Task.Run(async () =>
            {
                await this._messageHandler.ReceiveAsync(context, msg);
            }).AnyContext();
        }

        public override void OnException(ISocketContext<AmpMessage> context, Exception ex)
        {
            this._logger.LogError(ex, "server error occ");
            base.OnException(context, ex);
        }

        public override void OnDisconnected(ISocketContext<AmpMessage> context)
        {
            this._logger.LogInformation("client disconnected from {address}", context.RemoteEndPoint.Address);
            base.OnDisconnected(context);
        }

        public override void OnConnected(ISocketContext<AmpMessage> context)
        {
            this._logger.LogInformation("client connected from {address}", context.RemoteEndPoint.Address);
            base.OnConnected(context);
        }
    }
}
