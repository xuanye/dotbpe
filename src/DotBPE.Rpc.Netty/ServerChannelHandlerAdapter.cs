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
            Logger.Debug("读取消息");
            Task.Factory.StartNew(() =>
            {
                this._bootstrap.ChannelRead(context, msg);
            });

            Logger.Debug("异步读取消息完成");
        }
        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            Logger.Error(ex,$"与客户端：{context.Channel.RemoteAddress}通信时发送了错误");
            context.CloseAsync(); //关闭连接
        }
    }
}
