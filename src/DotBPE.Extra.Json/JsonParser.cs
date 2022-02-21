using System;
using System.Text;
using DotBPE.Rpc;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotBPE.Extra
{
    public class TextJsonParser : IJsonParser
    {
        static readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
           IgnoreNullValues =true            
        };
        public string ToJson(object item)
        {
            return JsonSerializer.Serialize(item, serializeOptions);
        }

        public string ToJson<T>(T item) where T : class
        {
            return JsonSerializer.Serialize<object>(item, serializeOptions);
        }

        public object FromJson(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, serializeOptions);
        }

        public T FromJson<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, serializeOptions);
        }

    }
}
