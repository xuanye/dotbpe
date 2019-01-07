using System;
using System.Text;
using DotBPE.Rpc;
using Newtonsoft.Json;

namespace DotBPE.Extra
{
    public class JsonSerializer:ISerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
            var json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public byte[] Serialize<T>(T item)
        {
            if (item == null)
            {
                return null;
            }
            var json = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(json);
        }

        public object Deserialize(byte[] data, Type type)
        {
            if (data == null)
            {
                return null;
            }
            var json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(json, type);
        }

        public byte[] Serialize(object item)
        {
            if (item == null)
            {
                return null;
            }
            var json = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(json);
        }

        public byte CodecType => (byte)Rpc.Codec.CodecType.JSON;
    }
}
