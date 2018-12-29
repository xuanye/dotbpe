using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
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
        public AmpRpcService(IServerMessageHandler<AmpMessage> messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override void OnReceive(ISocketContext<AmpMessage> context, AmpMessage msg)
        {
            Task.Run(async () =>
            {
                await _messageHandler.ReceiveAsync(context, msg);
            }).AnyContext();
        }

        public override void OnException(ISocketContext<AmpMessage> context, Exception ex)
        {
            base.OnException(context, ex);
        }

        public override void OnDisconnected(ISocketContext<AmpMessage> context)
        {
            base.OnDisconnected(context);
        }

        public override void OnConnected(ISocketContext<AmpMessage> context)
        {
            base.OnConnected(context);
        }
    }
}
