using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocol
{
    public class AmpDecodeHandler : DotNetty.Codecs.ByteToMessageDecoder
    {
        private readonly ISerializer _serializer;

        public AmpDecodeHandler(ISerializer serializer)
        {
            this._serializer = serializer;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes == 0)
            {
                return;
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
            msg.InvokeMessageType = (InvokeMessageType)Enum.ToObject(typeof(InvokeMessageType), type);


            msg.ServiceId = msg.Version == 0 ? input.ReadUnsignedShort() : input.ReadInt();


            msg.MessageId = input.ReadUnsignedShort();
            msg.Code = input.ReadInt();

            if (msg.Version == 1)
            {
                var codeType = input.ReadByte();
                if (codeType != this._serializer.CodecType)
                {
                    throw new RpcCodecException($"CodecType:{codeType} is not Match {this._serializer.CodecType}");
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
                if (left > input.ReadableBytes)
                {
                    throw new RpcCodecException("message not long enough!");
                }
                msg.Data = new byte[left];
                input.ReadBytes(msg.Data);
            }
            output.Add(msg);
        }
    }
}
