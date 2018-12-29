using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peach
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketContext<TMessage> where TMessage : Messaging.IMessage
    {
        
  
        /// <summary>
        /// get the connection id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Local Address
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Remote Address
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 内部Netty的Channel信息
        /// </summary>
        IChannel Channel { get; }


        /// <summary>
        /// 是否激活和活跃
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        Task SendAsync(TMessage message);
    }
}
