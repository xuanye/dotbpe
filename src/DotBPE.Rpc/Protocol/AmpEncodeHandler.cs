using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocol
{
    public class AmpEncodeHandler : DotNetty.Codecs.MessageToByteEncoder<AmpMessage>
    {
        private readonly ISerializer _serializer;

        public AmpEncodeHandler(ISerializer serializer)
        {
            this._serializer = serializer;
        }

        protected override void Encode(IChannelHandlerContext context, AmpMessage message, IByteBuffer output)
        {
            output.WriteByte(message.Version);
            output.WriteInt(message.Length);
            output.WriteInt(message.Sequence);
            output.WriteByte((byte)message.InvokeMessageType);

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
                output.WriteByte(_serializer.CodecType);
            }

            if (message.Data != null)
            {
                output.WriteBytes(message.Data);
            }
        }
    }
}
