using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Peach.Messaging;

namespace Peach
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketService<TMessage> where TMessage : IMessage
    {

        /// <summary>
        /// 当收到客户端消息时
        /// </summary>
        /// <param name="context">连接上下文信息</param>
        /// <param name="msg">消息内容</param>
        void OnReceive(ISocketContext<TMessage> context, TMessage msg);


        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="context">链接上下文</param>
        void OnConnected(ISocketContext<TMessage> context);

       

        /// <summary>
        /// 当socket连接断开时，会调用此方法
        /// </summary>
        /// <param name="context">链接上下文</param>
        void OnDisconnected(ISocketContext<TMessage> context);

        /// <summary>
        /// 当发生异常时，会调用此方法
        /// </summary>
        /// <param name="context">链接上下文</param>
        /// <param name="ex">异常信息</param>
        void OnException(ISocketContext<TMessage> context, Exception ex);
    }
}
