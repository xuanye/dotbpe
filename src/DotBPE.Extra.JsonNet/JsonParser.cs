using System;
using System.Text;
using DotBPE.Rpc;
using Newtonsoft.Json;

namespace DotBPE.Extra
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
