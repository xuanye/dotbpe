using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocol;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Protocols
{
    public class SumRes
    {
        public int Total { get; set; }
    }

    public class SumReq
    {
        public int A { get; set; }
        public int B { get; set; }
    }

    public class MessagePackTansportMessage: ITransportMessage
    {
        public CodecType CodecType { get { return CodecType.MessagePack; } } 

        public T Decode<T>(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<T>(buffer);
        }

        public byte[] Encode<T>(T message)
        {
            return MessagePackSerializer.Serialize(message);
        }
    }
}
