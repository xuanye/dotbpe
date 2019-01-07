using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using Peach.Buffer;
using Peach.Protocol;

namespace DotBPE.Rpc.Protocol
{
    /// <summary>
    /// Amp Protocol
    /// </summary>
    public class AmpProtocol : IProtocol<AmpMessage>
    {
        private readonly ISerializer _serializer;

        public AmpProtocol(ISerializer serializer)
        {
            this._serializer = serializer;
        }

        static readonly ProtocolMeta AMP_PROTOCOL_META = new ProtocolMeta
        {
            InitialBytesToStrip = 0, //读取时需要跳过的字节数
            LengthAdjustment = -5, //包实际长度的纠正，如果包长包括包头和包体，则要减去Length之前的部分
            LengthFieldLength = 4, //长度字段的字节数 整型为4个字节
            LengthFieldOffset = 1, //长度属性的起始（偏移）位
            MaxFrameLength = int.MaxValue,
            HeartbeatInterval = 60 * 1000 // 30秒没消息发一个心跳包
        };

        public ProtocolMeta GetProtocolMeta()
        {
            return AMP_PROTOCOL_META;
        }

        public void Pack(IBufferWriter writer, AmpMessage message)
        {
            writer.WriteByte(message.Version);
            writer.WriteInt(message.Length);
            writer.WriteInt(message.Sequence);
            writer.WriteByte((byte)message.InvokeMessageType);

            if (message.Version == 0)
            {
                writer.WriteUShort((ushort)message.ServiceId);
            }
            else
            {
                writer.WriteInt(message.ServiceId);
            }


            writer.WriteUShort(message.MessageId);

            writer.WriteInt(message.Code);

            if(message.Version == 1)
            {
                writer.WriteByte(_serializer.CodecType);
            }

            if (message.Data != null)
            {
                writer.WriteBytes(message.Data);
            }
        }

        public AmpMessage Parse(IBufferReader reader)
        {
            if (reader.ReadableBytes == 0)
            {
                return null;
            }

            var msg = new AmpMessage {Version = reader.ReadByte()};

            int headLength;
            if (msg.Version == 0 )
            {
                headLength = AmpMessage.VERSION_0_HEAD_LENGTH;
                if (reader.ReadableBytes < AmpMessage.VERSION_0_HEAD_LENGTH - 1)
                {
                    throw new RpcCodecException($"decode error ,ReadableBytes={reader.ReadableBytes+1},HEAD_LENGTH={AmpMessage.VERSION_0_HEAD_LENGTH}");
                }
            }
            else if (msg.Version == 1 )
            {
                headLength = AmpMessage.VERSION_1_HEAD_LENGTH;
                if (reader.ReadableBytes < AmpMessage.VERSION_1_HEAD_LENGTH - 1)
                {
                    throw new RpcCodecException($"decode error ,ReadableBytes={reader.ReadableBytes+1},HEAD_LENGTH={AmpMessage.VERSION_1_HEAD_LENGTH}");
                }
            }
            else
            {
                throw new RpcCodecException($"decode error ,{msg.Version} is not support");
            }

            var length = reader.ReadInt();
            msg.Sequence = reader.ReadInt();
            var type = reader.ReadByte();
            msg.InvokeMessageType = (InvokeMessageType)Enum.ToObject(typeof(InvokeMessageType), type);


            msg.ServiceId = msg.Version == 0 ? reader.ReadUShort() : reader.ReadInt();


            msg.MessageId = reader.ReadUShort();
            msg.Code = reader.ReadInt();

            if (msg.Version == 1)
            {
                byte codeType = reader.ReadByte();
                if (codeType != this._serializer.CodecType)
                {
                    throw  new RpcCodecException($"CodecType:{codeType} is not Match {this._serializer.CodecType}");
                }
                msg.CodecType = (CodecType)Enum.ToObject(typeof(CodecType), codeType);
            }
            else
            {
                msg.CodecType = CodecType.Protobuf;
            }

            int left = length - headLength;
            if (left > 0)
            {
                if (left > reader.ReadableBytes)
                {
                    throw new RpcCodecException("message not long enough!");
                }
                msg.Data = new byte[left];
                reader.ReadBytes(msg.Data);
            }
            return msg;
        }
    }
}
