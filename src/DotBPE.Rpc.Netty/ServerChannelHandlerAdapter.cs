using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc.Netty
{
    public class ServerChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage:IMessage
    {
        private readonly NettyServerBootstrap<TMessage> _bootstrap;
        private readonly ILogger _logger;
        public ServerChannelHandlerAdapter(NettyServerBootstrap<TMessage> bootstrap, ILogger logger):base(false)
        {
            this._bootstrap = bootstrap;
            this._logger = logger;
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端连接上了{context.Channel.RemoteAddress}");
            base.ChannelActive(context);
        }
        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            Console.WriteLine("收到一条消息...");
            Task.Run(() =>
            {
                this._bootstrap.ChannelRead(context, msg);
            });           
        }
        
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
           
            Console.WriteLine("Exception: " + exception);
            _logger.LogError($"与客户端：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
        }
    }
}
