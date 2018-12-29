using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Peach.Diagnostics;

namespace Peach.Tcp
{
    public class TcpServerChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage : Messaging.IMessage
    {
        private static DiagnosticListener listener = new DiagnosticListener(Diagnostics.DiagnosticListenerExtensions.DiagnosticListenerName);
        private readonly ISocketService<TMessage> _service;
        private readonly Protocol.IProtocol<TMessage> _protocol;
        public TcpServerChannelHandlerAdapter(
            ISocketService<TMessage> service,
            Protocol.IProtocol<TMessage> protocol
        ) : base(true)
        {
            this._service = service;
            this._protocol = protocol;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _service.OnConnected(new SocketContext<TMessage>(context.Channel, this._protocol));
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _service.OnDisconnected(new SocketContext<TMessage>(context.Channel, this._protocol));
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            listener.ServiceReceive(msg);
            _service.OnReceive(new SocketContext<TMessage>(context.Channel, this._protocol), msg);
            listener.ServiceReceiveCompleted(msg);
        }

        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            this._service.OnException(new SocketContext<TMessage>(context.Channel, this._protocol), ex);
            listener.ServiceException(ex);
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
                    context.CloseAsync(); //关闭连接
                }
            }
        }
    }
}