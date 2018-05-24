using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protobuf
{
    public interface IProtobufDescriptorFactory
    {
        /// <summary>
        /// 获取请求消息的消息定义
        /// </summary>
        /// <param name="serviceId">服务ID</param>
        /// <param name="messageId">消息ID</param>
        /// <returns>消息描述对象</returns>
        MessageDescriptor GetRequestDescriptor(int serviceId, int messageId);

        /// <summary>
        /// 获取响应消息的消息定义
        /// </summary>
        /// <param name="serviceId">The service Id.</param>
        /// <param name="messageId">The message Id.</param>
        /// <returns>消息描述对象</returns>
        MessageDescriptor GetResponseDescriptor(int serviceId, int messageId);
    }
}
