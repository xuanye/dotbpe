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
        static readonly ConcurrentDictionary<Type, MessageParser> PARSER_CACHE = new ConcurrentDictionary<Type, MessageParser>();
        private static readonly Type BaseType = typeof(IMessage);

        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
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

                var property = messageType.GetProperty("Parser");
                if(property != null)
                {
                    return (MessageParser)property.GetValue(null);
                }
            }
            throw new Rpc.Exceptions.RpcException("Message is not a Protobuf Message");
        }

        public object Deserialize(byte[] data, Type messageType)
        {
            if(data == null)
            {
                return null;
            }
            if (!PARSER_CACHE.TryGetValue(messageType,out var parser))
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
                return message == null ? new byte[0] : message.ToByteArray();
            }
            throw new Rpc.Exceptions.RpcException("Message is not a Protobuf Message");
        }

        public byte CodecType => (byte)Rpc.Codec.CodecType.Protobuf;
    }
}
