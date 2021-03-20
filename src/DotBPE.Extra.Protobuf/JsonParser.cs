using System;
using System.Collections.Concurrent;
using DotBPE.Rpc;
using DotBPE.Rpc.BestPractice;
using Google.Protobuf;

namespace DotBPE.Extra
{
    public class JsonParser:IJsonParser
    {
        private static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));
        static readonly ConcurrentDictionary<Type, MessageParser> PARSER_CACHE = new ConcurrentDictionary<Type, MessageParser>();
        private static readonly Type BaseType = typeof(IMessage);

        private static MessageParser FindMessageParser(Type messageType)
        {
            if (BaseType.IsAssignableFrom(messageType) && messageType.IsClass)
            {
                var property = messageType.GetProperty("Parser");
                if (property != null)
                {
                    return (MessageParser)property.GetValue(null);
                }
            }
            throw new Rpc.Exceptions.RpcException("Message is not a Protobuf Message");
        }

        public string ToJson(object item)
        {
            
            if(item is IMessage message)
            {
                return AmpJsonFormatter.Format(message);
            }
            return null;
        }

        public string ToJson<T>(T item) where T : class
        {
          

            if (item is IMessage message)
            {
                return AmpJsonFormatter.Format(message);
            }
            return null;
        }

        public object FromJson(string json, Type type)
        {
            if (!PARSER_CACHE.TryGetValue(type, out var parser))
            {
                parser = FindMessageParser(type);
                PARSER_CACHE.TryAdd(type, parser);
            }
            return parser.ParseJson(json);
        }

        public T FromJson<T>(string json) where T : class
        {
            var type = typeof(T);
            return (T)FromJson(json, type);
        }
       
    }
}
