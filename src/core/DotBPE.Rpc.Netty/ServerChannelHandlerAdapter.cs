using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using DotNetty.Handlers.Timeout;

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
            Logger.Debug($"client {context.Channel.RemoteAddress} connected");
            base.ChannelActive(context);
        }
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Logger.Debug($"client {context.Channel.RemoteAddress} disconnected");
            base.ChannelInactive(context);
        }
        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            Logger.Debug("ready to read message");

            this._bootstrap.ChannelRead(context, msg).ContinueWith( (task)=>{
               Logger.Debug("read message completed");
            });

        }
        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            Logger.Error(ex,$"client：{context.Channel.RemoteAddress} occur an exception ");
            context.CloseAsync(); //关闭连接
        }
        //服务端超时则直接关闭链接
        public override void UserEventTriggered(IChannelHandlerContext context, object evt){
            if(evt is IdleStateEvent){
                var eventState = evt as IdleStateEvent;
                if(eventState !=null){
                    Logger.Error($"client {context.Channel.Id},{context.Channel.RemoteAddress} is timeout，close it!");
                    context.CloseAsync(); //关闭连接
                }
            }
        }
    }
}
