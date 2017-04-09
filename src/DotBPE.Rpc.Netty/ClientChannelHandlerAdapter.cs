using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.Netty
{
    public class ClientChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage :InvokeMessage
    {
        private readonly NettyClientBootstrap<TMessage> _bootstrap;
        private readonly ILogger _logger;

        public ClientChannelHandlerAdapter(NettyClientBootstrap<TMessage> bootstrap,ILogger logger)
        {
            this._bootstrap = bootstrap;
            this._logger = logger;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("连接已经建立...");
            base.ChannelActive(context);
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            // 这里应该移除之前ITransprotFactory中的缓存 或者通知对方
            this._bootstrap.OnChannelInactive(context);
        }
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, TMessage msg)
        {
            Console.WriteLine("收到一条消息...");
            this._bootstrap.ChannelRead(ctx, msg);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
        }
    }
}
