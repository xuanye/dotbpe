using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Netty
{
    public class ServerHandlerAdapter : SimpleChannelInboundHandler<IMessage>
    {
        private readonly Action<IChannelHandlerContext, IMessage> _recieveAction;
        private readonly ILogger _logger;
        public ServerHandlerAdapter(Action<IChannelHandlerContext, IMessage> recieveAction, ILogger logger):base(false)
        {
            this._recieveAction = recieveAction;
            this._logger = logger;
        }
        protected override void ChannelRead0(IChannelHandlerContext context, IMessage msg)
        {
            Task.Run(() =>
            {
                _recieveAction(context, msg);
            });           
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError($"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
        }
    }
}
