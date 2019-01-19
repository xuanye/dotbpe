using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Protocol
{
    /// <summary>
    /// 协议接口
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IProtocol<TMessage>
        where TMessage :  Messaging.IMessage
    {
        ProtocolMeta GetProtocolMeta();

        TMessage Parse(Buffer.IBufferReader reader);

        void Pack(Buffer.IBufferWriter writer, TMessage message);
    
    }
}
