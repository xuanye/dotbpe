using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
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
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public override void OnReceive(ISocketContext<AmpMessage> context, AmpMessage msg)
        {
            _logger.LogInformation("receive message {id}", msg.Id);
            Task.Run(async () =>
            {
                await _messageHandler.ReceiveAsync(context, msg);
            }).AnyContext();
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
