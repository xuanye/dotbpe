using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IMessageCodecs<TMessage> where TMessage : IMessage
    {
        byte[] Encode(TMessage message, IBufferWriter writer);
        IMessage Decode(IBufferReader reader);
    }

    public interface IMessage
    {
        int Length { get; }
    }
}
