// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocols
{
    /// <summary>
    /// Amp Protocol
    /// </summary>
    public static class AmpProtocol
    {
        public const int InitialBytesToStrip = 0; //Number of bytes to be skipped when reading
        public const int LengthAdjustment = -5; //Correction of the actual length of the package, minus the part before Length if the package length includes the header and the package body
        public const int LengthFieldLength = 4;//Number of bytes in the length field Integer is 4 bytes
        public const int LengthFieldOffset = 1;//Start (offset) bit of the length attribute
        public const int MaxFrameLength = int.MaxValue;
        public const int HeartbeatInterval = 15; // Send a heartbeat if there is no new message for 15 seconds


        public static AmpMessage? Decode(CodecType codecType, IByteBuffer input)
        {

            if (input.ReadableBytes == 0)
            {
                return null;
            }

            var msg = new AmpMessage { Version = input.ReadByte() };

            int headLength;
            if (msg.Version == 0)
            {
                headLength = AmpMessage.VERSION_0_HEAD_LENGTH;
                if (input.ReadableBytes < AmpMessage.VERSION_0_HEAD_LENGTH - 1)
                {
                    throw new RpcCodecException($"decode error ,ReadableBytes={input.ReadableBytes + 1},HEAD_LENGTH={AmpMessage.VERSION_0_HEAD_LENGTH}");
                }
            }
            else if (msg.Version == 1)
            {
                headLength = AmpMessage.VERSION_1_HEAD_LENGTH;
                if (input.ReadableBytes < AmpMessage.VERSION_1_HEAD_LENGTH - 1)
                {
                    throw new RpcCodecException($"decode error ,ReadableBytes={input.ReadableBytes + 1},HEAD_LENGTH={AmpMessage.VERSION_1_HEAD_LENGTH}");
                }
            }
            else
            {
                throw new RpcCodecException($"decode error ,{msg.Version} is not support");
            }

            var length = input.ReadInt();
            msg.Sequence = input.ReadInt();
            var type = input.ReadByte();
            msg.MessageType = (RpcMessageType)Enum.ToObject(typeof(RpcMessageType), type);


            msg.ServiceId = msg.Version == 0 ? input.ReadUnsignedShort() : input.ReadInt();


            msg.MessageId = input.ReadUnsignedShort();
            msg.Code = input.ReadInt();

            if (msg.Version == 1)
            {
                var msgCodecType = input.ReadByte();
                if (msgCodecType != (byte)codecType)
                {
                    throw new RpcCodecException($"CodecType:{msgCodecType} is not match {codecType}");
                }
                msg.CodecType = codecType;
            }
            else
            {
                msg.CodecType = CodecType.Protobuf;
            }

            int left = length - headLength;
            if (left > 0)
            {
                if (left > input.ReadableBytes)
                {
                    throw new RpcCodecException("message not long enough!");
                }
                msg.Data = new byte[left];
                input.ReadBytes(msg.Data);
            }
            return msg;
        }


        public static void Encode(AmpMessage message, CodecType codecType, IByteBuffer output)
        {
            output.WriteByte(message.Version);
            output.WriteInt(message.Length);
            output.WriteInt(message.Sequence);
            output.WriteByte((byte)message.MessageType);

            if (message.Version == 0)
            {
                output.WriteUnsignedShort((ushort)message.ServiceId);
            }
            else
            {
                output.WriteInt(message.ServiceId);
            }

            output.WriteUnsignedShort(message.MessageId);

            output.WriteInt(message.Code);

            if (message.Version == 1)
            {
                output.WriteByte((byte)codecType);
            }

            if (message.Data != null)
            {
                output.WriteBytes(message.Data);
            }
        }

    }
}
