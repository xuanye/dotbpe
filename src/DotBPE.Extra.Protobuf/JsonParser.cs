// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;

namespace DotBPE.Extra
{
    public class JsonParser : IJsonParser
    {
        private static readonly JsonFormatter _ampJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));
        private static readonly ConcurrentDictionary<Type, MessageParser> _parseCache = new ConcurrentDictionary<Type, MessageParser>();
        private static readonly Type _baseType = typeof(IMessage);

        private static MessageParser FindMessageParser(Type messageType)
        {
            if (_baseType.IsAssignableFrom(messageType) && messageType.IsClass)
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

            if (item is IMessage message)
            {
                return _ampJsonFormatter.Format(message);
            }
            return null;
        }

        public string ToJson<T>(T item) where T : class
        {


            if (item is IMessage message)
            {
                return _ampJsonFormatter.Format(message);
            }
            return null;
        }

        public object FromJson(string json, Type type)
        {
            if (!_parseCache.TryGetValue(type, out var parser))
            {
                parser = FindMessageParser(type);
                _parseCache.TryAdd(type, parser);
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
