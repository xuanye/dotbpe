using DotBPE.Rpc.Codes;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Rpc.Netty
{

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="DotNetty.Transport.Channels.SimpleChannelInboundHandler{TMessage}" />
    public class ServerChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage : InvokeMessage
    {
        private readonly NettyServerBootstrap<TMessage> _bootstrap;
        private readonly ILogger Logger;

        public ServerChannelHandlerAdapter(NettyServerBootstrap<TMessage> bootstrap,ILoggerFactory factory) : base(true)
        {
            this._bootstrap = bootstrap;
            this.Logger = factory.CreateLogger(this.GetType());
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logger.LogDebug($"client {context.Channel.RemoteAddress} connected");
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Logger.LogDebug($"client {context.Channel.RemoteAddress} disconnected");
            base.ChannelInactive(context);
        }

        protected async override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            Logger.LogDebug("ready to read message");
            await this._bootstrap.ChannelRead(context, msg);
        }

        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            Logger.LogError(ex, "client：{remoteAddress} occur an exception ", context.Channel.RemoteAddress);
            context.CloseAsync(); //关闭连接
        }

        //服务端超时则直接关闭链接
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                var eventState = evt as IdleStateEvent;
                if (eventState != null)
                {
                    Logger.LogError("client {channelId},{remoteAddress} is timeout，close it!", context.Channel.Id,context.Channel.RemoteAddress);
                    context.CloseAsync(); //关闭连接
                }
            }
        }
    }
}
