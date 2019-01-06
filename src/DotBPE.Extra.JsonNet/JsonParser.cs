using System;
using System.Text;
using DotBPE.Rpc;
using Newtonsoft.Json;

namespace DotBPE.Extra
{
    public class JsonParser:IJsonParser
    {
        public string ToJson(object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public string ToJson<T>(T item) where T : class
        {
            return  JsonConvert.SerializeObject(item);
        }

        public object FromJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
