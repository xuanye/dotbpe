using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace DotBPE.Extra
{
    public class MessagePackSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(bytes);
        }

        public object Deserialize(byte[] data, Type type)
        {
            return MessagePack.MessagePackSerializer.NonGeneric.Deserialize(type, data);
        }

        public byte[] Serialize<T>(T item)
        {
            return MessagePack.MessagePackSerializer.Serialize(item);
        }

        public byte[] Serialize(object item)
        {
            return MessagePack.MessagePackSerializer.NonGeneric.Serialize(item.GetType(),item);
        }

        public byte CodecType => (byte)Rpc.Codec.CodecType.MessagePack;
    }
}
