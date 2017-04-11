using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.Netty
{
    public class ServerChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage:InvokeMessage
    {
        private readonly NettyServerBootstrap<TMessage> _bootstrap;

        static readonly ILogger Logger = Environment.Logger.ForType<ServerChannelHandlerAdapter<TMessage>>();

        public ServerChannelHandlerAdapter(NettyServerBootstrap<TMessage> bootstrap):base(true)
        {
            this._bootstrap = bootstrap;           
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logger.Debug($"客户端连接上了{context.Channel.RemoteAddress}");
            base.ChannelActive(context);
        }
        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            Logger.Debug("收到一条消息...");
            Task.Run(() =>
            {
                this._bootstrap.ChannelRead(context, msg);
            });           
        }
        
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {  
            Logger.Error($"与客户端：{context.Channel.RemoteAddress}通信时发送了错误。", exception);

        }
    }
}
