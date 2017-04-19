using System;
using DotBPE.Rpc.Codes;
using DotNetty.Transport.Channels;
using DotBPE.Rpc.Logging;
using DotNetty.Handlers.Timeout;

namespace DotBPE.Rpc.Netty
{
    public class ClientChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage :InvokeMessage
    {
        private readonly NettyClientBootstrap<TMessage> _bootstrap;
        static readonly ILogger Logger = Environment.Logger.ForType<ClientChannelHandlerAdapter<TMessage>>();

        public ClientChannelHandlerAdapter(NettyClientBootstrap<TMessage> bootstrap)
        {
            this._bootstrap = bootstrap;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logger.Debug($"服务端{context.Channel.RemoteAddress}连接成功");
            base.ChannelActive(context);
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            // 这里应该移除之前ITransprotFactory中的缓存 或者通知对方
            Logger.Debug($"服务端{context.Channel.RemoteAddress}断开连接");
            this._bootstrap.OnChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMessage msg)
        {
            this._bootstrap.ChannelRead(ctx, msg);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            Logger.Error(ex,$"服务端{context.Channel.RemoteAddress}通信时发送了错误");
            context.CloseAsync(); //关闭连接
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt){
            if(evt is IdleStateEvent){
                var eventState = evt as IdleStateEvent;
                if(eventState !=null){
                    this._bootstrap.SendHeartbeatAsync(context,eventState);
                }
            }
        }
    }
}
