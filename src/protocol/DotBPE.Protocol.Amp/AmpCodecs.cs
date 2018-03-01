#region copyright

// -----------------------------------------------------------------------
//  <copyright file="AmpCodecs.cs” project="DotBPE.Protocol.Amp">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 16:47
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using DotBPE.Rpc.Codes;

namespace DotBPE.Protocol.Amp
{
    public class AmpCodecs : IMessageCodecs<AmpMessage>
    {
        private static readonly MessageMeta AmpMeta = new MessageMeta
        {
            InitialBytesToStrip = 0, //读取时需要跳过的字节数
            LengthAdjustment = -5, //包实际长度的纠正，如果包长包括包头和包体，则要减去Length之前的部分
            LengthFieldLength = 4, //长度字段的字节数 整型为4个字节
            LengthFieldOffset = 1, //长度属性的起始（偏移）位
            MaxFrameLength = int.MaxValue,
            HeartbeatInterval = 60 * 1000 // 30秒没消息发一个心跳包
        };

        public void Encode(AmpMessage message, IBufferWriter writer)
        {
            writer.WriteByte(message.Version);
            writer.WriteInt(message.Length);
            writer.WriteInt(message.Sequence);
            writer.WriteByte((byte)message.InvokeMessageType);
            writer.WriteUShort(message.ServiceId);
            writer.WriteUShort(message.MessageId);

            writer.WriteInt(message.Code);

            if (message.Data != null)
            {
                writer.WriteBytes(message.Data);
            }
        }

        public AmpMessage Decode(IBufferReader reader)
        {
            if (reader.ReadableBytes == 0)
            {
                return null;
            }
            AmpMessage msg = new AmpMessage();
            if (reader.ReadableBytes < AmpMessage.HEAD_LENGTH)
            {
                throw new Rpc.Exceptions.RpcCodecException($"decode error ,ReadableBytes={reader.ReadableBytes},HEAD_LENGTH={AmpMessage.HEAD_LENGTH}");
            }
            msg.Version = reader.ReadByte();
            int length = reader.ReadInt();
            msg.Sequence = reader.ReadInt();
            byte type = reader.ReadByte();
            msg.InvokeMessageType = Rpc.Utils.ParseUtils.ParseMessageType(type);
            msg.ServiceId = reader.ReadUShort();
            msg.MessageId = reader.ReadUShort();
            msg.Code = reader.ReadInt();

            int left = length - AmpMessage.HEAD_LENGTH;
            if (left > 0)
            {
                if (left > reader.ReadableBytes)
                {
                    throw new Rpc.Exceptions.RpcCodecException("消息长度不正确");
                }
                msg.Data = new byte[left];
                reader.ReadBytes(msg.Data);
            }
            return msg;
        }


        public MessageMeta GetMessageMeta()
        {
            return AmpMeta;
        }

        public AmpMessage HeartbeatMessage()
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(0, 0);
            return message;
        }
    }
}
