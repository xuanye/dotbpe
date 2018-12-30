using DotBPE.Rpc;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Extra
{
    public class ProtobufSerializer : ISerializer
    {
        static ConcurrentDictionary<Type, MessageParser> PARSER_CACHE = new ConcurrentDictionary<Type, MessageParser>();
        static Type BaseType = typeof(IMessage);

        public T Deserialize<T>(byte[] data)
        {
            var messageType = typeof(T);
            return (T)Deserialize( data,messageType);
        }

        public byte[] Serialize<T>(T item)
        {
            return Serialize(typeof(T), item);
        }

        private static MessageParser FindMessageParser(Type messageType)
        {
            if (BaseType.IsAssignableFrom(messageType) && messageType.IsClass)
            {
               var descriptorType = messageType.GetProperty("Descriptor").PropertyType;
               var parserType = descriptorType.GetProperty("Parser").PropertyType;
               return (MessageParser)Activator.CreateInstance(parserType);
            }
            throw new Rpc.Exceptions.RpcException("Message is not a Protobuf Message");
        }

        public object Deserialize(byte[] data, Type messageType)
        {
            MessageParser parser;
            if (!PARSER_CACHE.TryGetValue(messageType,out parser))
            {
                parser = FindMessageParser(messageType);
                PARSER_CACHE.TryAdd(messageType, parser);
            }
            return parser.ParseFrom(data);
        }

        public byte[] Serialize(object item)
        {
            return Serialize(item.GetType(), item);
        }

        private byte[] Serialize(Type messageType, object item)
        {
            if (BaseType.IsAssignableFrom(messageType) && messageType.IsClass)
            {
                IMessage message =item as IMessage;
                if(message == null)
                {
                    return new byte[0];
                }
                return message.ToByteArray();
            }
            throw new Rpc.Exceptions.RpcException("Message is not a Protobuf Message");
        }
    }
}
