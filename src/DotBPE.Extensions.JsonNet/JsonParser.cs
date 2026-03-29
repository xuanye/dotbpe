using System;
using System.Text;
using DotBPE.Rpc;
using Newtonsoft.Json;

namespace DotBPE.Extensions
{
    public class JsonParser:IJsonParser
    {
        static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        public string ToJson(object item)
        {
            return JsonConvert.SerializeObject(item,Formatting.None,settings);
        }

        public string ToJson<T>(T item) where T : class
        {
            return  JsonConvert.SerializeObject(item,Formatting.None,settings);
        }

        public object FromJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type,settings);
        }

        public T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json,settings);
        }

    }
}

namespace DotBPE.Extra
{
    /// <summary>
    /// JsonParser is obsolete. Use DotBPE.Extensions.JsonParser instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.JsonParser instead")]
    public class JsonParser : DotBPE.Extensions.JsonParser
    {
    }
}
