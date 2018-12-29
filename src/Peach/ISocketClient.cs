using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using Peach.EventArgs;
using Peach.Messaging;

namespace Peach
{
    public interface ISocketClient<TMessage> where TMessage : IMessage
    {

        /// <summary>
        /// Connect Async
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        Task<ISocketContext<TMessage>> ConnectAsync(EndPoint endPoint);

       
        /// <summary>
        /// Receive Message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        void RaiseReceive(ISocketContext<TMessage> context, TMessage msg);

        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="context"></param>
        void RaiseConnected(ISocketContext<TMessage> context);

        /// <summary>
        /// 当socket连接断开时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void RaiseDisconnected(ISocketContext<TMessage> context);

        /// <summary>
        /// 当发生异常时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void RaiseError(ISocketContext<TMessage> context, Exception ex);

        /// <summary>
        /// 需要发送心跳包
        /// </summary>
        /// <param name="context"></param>
        /// <param name="eventState"></param>
        void RaiseIdleState(SocketContext<TMessage> context, IdleStateEvent eventState);


        event EventHandler<MessageReceivedEventArgs<TMessage>> OnReceived;
        event EventHandler<ErrorEventArgs<TMessage>> OnError;
        event EventHandler<ConnectedEventArgs<TMessage>> OnConnected;
        event EventHandler<DisconnectedEventArgs<TMessage>> OnDisconnected;
        event EventHandler<IdleStateEventArgs<TMessage>> OnIdleState;
    }
}
