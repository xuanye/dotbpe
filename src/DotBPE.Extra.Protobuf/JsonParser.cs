using System;
using DotBPE.Rpc;

namespace DotBPE.Extra
{
    public class JsonParser:IJsonParser
    {
        public string ToJson(object item)
        {
            throw new NotImplementedException();
        }

        public string ToJson<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public object FromJson(string json, Type type)
        {
            throw new NotImplementedException();
        }

        public T FromJson<T>(string json)
        {
            throw new NotImplementedException();
        }
    }
}
