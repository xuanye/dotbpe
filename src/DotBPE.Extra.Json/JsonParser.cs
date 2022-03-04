using System;
using System.Text;
using DotBPE.Rpc;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotBPE.Extra
{
    public class TextJsonParser : IJsonParser
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            IgnoreReadOnlyProperties = false,
            AllowTrailingCommas = false
        };
      
        public string ToJson(object item)
        {
            return JsonSerializer.Serialize(item, JsonSerializerOptions);
        }

        public string ToJson<T>(T item) where T : class
        {
            return JsonSerializer.Serialize<object>(item, JsonSerializerOptions);
        }

        public object FromJson(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, JsonSerializerOptions);
        }

        public T FromJson<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }

    }
}
