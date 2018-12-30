using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Extra
{
    public class MessagePackSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(bytes);          
        }

        public byte[] Serialize<T>(T item)
        {
            return MessagePack.MessagePackSerializer.Serialize(item);
        }
    }
}
