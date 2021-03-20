using System;
using System.Text;
using System.Text.Json;
using DotBPE.Rpc;

namespace DotBPE.Extra
{
    public class TextJsonSerializer:ISerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                return default;
            }
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json);
        }

        public byte[] Serialize<T>(T item)
        {
            if (item == null)
            {
                return null;
            }
            return JsonSerializer.SerializeToUtf8Bytes(item);
        }

        public object Deserialize(byte[] data, Type type)
        {
            if (data == null)
            {
                return null;
            }
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize(json,type);
        }

        public byte[] Serialize(object item)
        {
            if (item == null)
            {
                return null;
            }
            return JsonSerializer.SerializeToUtf8Bytes(item);           
        }

        public byte CodecType => (byte)Rpc.Codec.CodecType.JSON;
    }
}
