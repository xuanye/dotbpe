#region copyright
// -----------------------------------------------------------------------
//  <copyright file="AvenueCodecs.cs” project="DotBPE.Protocol.Amp">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 17:16
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;

namespace DotBPE.Protocol.Avenue
{
    public class AvenueCodecs:IMessageCodecs<AvenueMessage>
    {
        private static readonly MessageMeta Meta = new MessageMeta
        {
            InitialBytesToStrip = 0,
            LengthAdjustment = 0,
            LengthFieldLength = 4,
            LengthFieldOffset = 1,
            MaxFrameLength = AvenueConstans.MAX_FRAME_SIZE
        };
        public void Encode(AvenueMessage message, IBufferWriter writer)
        {
            writer.WriteByte((byte)message.Flag);
            writer.WriteByte((byte)message.HeadLength);
            writer.WriteByte((byte)message.Version);
            writer.WriteByte(0x0F);

            writer.WriteInt(message.Length);
            writer.WriteInt(message.ServiceId);
            writer.WriteInt(message.MsgId);
            writer.WriteInt(message.Sequence);

            writer.WriteByte(0);
            writer.WriteByte((byte)message.MustReach);
            writer.WriteByte((byte)message.Format);
            writer.WriteByte((byte)message.Encoding);


            writer.WriteInt(0);

            writer.WriteBytes(AvenueConstans.EMPTY_SIGNATURE);

            if (message.XHead != null)
            {
                writer.WriteBytes(message.XHead);
            }

            if (message.Body != null)
            {
                writer.WriteBytes(message.Body);
            }
        }

        public AvenueMessage Decode(IBufferReader input)
        {
            var message = new AvenueMessage();
            //开始读取头
            message.Flag = input.ReadByte();
            if (message.Flag != AvenueConstans.TYPE_REQUEST && message.Flag != AvenueConstans.TYPE_RESPONSE)
            {
                throw new RpcCodecException("package_type_error");
            }

            var headlen = input.ReadByte();
            message.Version = input.ReadByte();
            if (message.Version != AvenueConstans.VERSION_1)
            {
                throw new RpcCodecException("package_version_error");
            }
            input.ReadByte();
            //包长
            int packetLen = input.ReadInt();
            int serviceId = input.ReadInt();
            if (serviceId < 0)
            {
                throw new RpcCodecException("package_serviceid_error");
            }
            int msgId = input.ReadInt();
            if (msgId != 0)
            {
                if (serviceId == 0)
                {
                    throw new RpcCodecException("package_msgid_error");
                }
            }
            message.ServiceId = serviceId;
            message.MsgId = msgId;
            message.Sequence = input.ReadInt();

            //下4位为 optional
            //context
            input.ReadByte();
            byte mustReach = input.ReadByte();
            byte format = input.ReadByte();
            byte encoding = input.ReadByte();

            if (mustReach != AvenueConstans.MUSTREACH_NO && mustReach != AvenueConstans.MUSTREACH_YES)
            {
                throw new RpcCodecException("package_mustreach_error");
            }

            if (format != AvenueConstans.FORMAT_TLV)
            {
                throw new RpcCodecException("package_format_error");
            }

            if (encoding != AvenueConstans.ENCODING_GBK && encoding != AvenueConstans.ENCODING_UTF8)
            {
                throw new RpcCodecException("package_encoding_error");
            }
            message.MustReach = mustReach;
            message.Format = format;
            message.Encoding = encoding;
            // 优先级实际没有用
            input.ReadInt();
            //signature
            input.ReadBytes(new byte[16]);


            if (headlen > AvenueConstans.STANDARD_HEADLEN)
            {
                message.XHead = new byte[headlen - AvenueConstans.STANDARD_HEADLEN];
                input.ReadBytes(message.XHead);
            }
            //开始读取Body
            message.Body = new byte[packetLen - headlen];
            input.ReadBytes(message.Body);

            return message;
        }

        public MessageMeta GetMessageMeta()
        {
            return Meta;
        }

        public AvenueMessage HeartbeatMessage()
        {
            throw new NotImplementedException();
        }
    }
}