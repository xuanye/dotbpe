using DotBPE.Rpc.Codes;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace DotBPE.Rpc.Netty
{
    public class ChannelDecodeHandler<TMessage> : DotNetty.Codecs.ByteToMessageDecoder where TMessage : InvokeMessage
    {
        private readonly IMessageCodecs<TMessage> _codecs;
       

        public ChannelDecodeHandler(IMessageCodecs<TMessage> codecs)
        {
            this._codecs = codecs;           
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            IBufferReader reader = NettyBufferManager.CreateBufferReader(input);
            InvokeMessage message = this._codecs.Decode(reader);
            if (message != null)
            {
                output.Add(message);
            }
            reader = null;
        }
    }
}
