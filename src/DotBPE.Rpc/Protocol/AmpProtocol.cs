using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using Peach.Buffer;
using Peach.Protocol;

namespace DotBPE.Rpc.Protocol
{
    /// <summary>
    /// TODO：待实现协议
    /// </summary>
    public class AmpProtocol : IProtocol<AmpMessage>
    {
        static ProtocolMeta AMP_PROTOCOL_META = new ProtocolMeta
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
            writer.WriteUShort(message.ServiceId);
            writer.WriteUShort(message.MessageId);

            writer.WriteInt(message.Code);

            if(message.Version == 1)
            {
                writer.WriteByte((byte)message.CodecType);
            }

            if (message.Data != null)
            {
                writer.WriteBytes(message.Data);
            }
        }

        public void PackHeartBeat(IBufferWriter writer)
        {
            AmpMessage message = new AmpMessage();
            message.ServiceId = 0;
            message.MessageId = 0;
        }

        public AmpMessage Parse(IBufferReader reader)
        {
            if (reader.ReadableBytes == 0)
            {
                return null;
            }
            AmpMessage msg = new AmpMessage();
            msg.Version = reader.ReadByte();

            int headLength= AmpMessage.VERSION_0_HEAD_LENGTH;
            if (msg.Version == 0 && reader.ReadableBytes < AmpMessage.VERSION_0_HEAD_LENGTH - 1)
            {
                headLength = AmpMessage.VERSION_0_HEAD_LENGTH;
                throw new RpcCodecException($"decode error ,ReadableBytes={reader.ReadableBytes+1},HEAD_LENGTH={AmpMessage.VERSION_0_HEAD_LENGTH}");
            }
            if (msg.Version == 1 && reader.ReadableBytes < AmpMessage.VERSION_1_HEAD_LENGTH - 1)
            {
                headLength = AmpMessage.VERSION_1_HEAD_LENGTH;
                throw new RpcCodecException($"decode error ,ReadableBytes={reader.ReadableBytes+1},HEAD_LENGTH={AmpMessage.VERSION_1_HEAD_LENGTH}");
            }
                        
            int length = reader.ReadInt();
            msg.Sequence = reader.ReadInt();
            byte type = reader.ReadByte();
            msg.InvokeMessageType = (InvokeMessageType)Enum.ToObject(typeof(InvokeMessageType), type);
            msg.ServiceId = reader.ReadUShort();
            msg.MessageId = reader.ReadUShort();
            msg.Code = reader.ReadInt();

            if (msg.Version == 1)
            {
                byte codeType = reader.ReadByte();
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
