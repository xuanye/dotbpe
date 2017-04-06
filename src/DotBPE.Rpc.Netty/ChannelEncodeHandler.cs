using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace DotBPE.Rpc.Netty
{
    public class ChannelEncodeHandler<TMessage> : MessageToByteEncoder<TMessage> where TMessage :IMessage
    {
        private readonly IMessageCodecs<TMessage> _codecs;
        private readonly NettyBufferManager bufferManager;
        

        public ChannelEncodeHandler(IMessageCodecs<TMessage> msgCodecs) {
            this._codecs = msgCodecs;
            this.bufferManager = new NettyBufferManager();
        }
        protected override void Encode(IChannelHandlerContext context, TMessage message, IByteBuffer output)
        {           
            IByteBuffer buffer = context.Allocator.Buffer(message.Length);
            IBufferWriter writer = this.bufferManager.CreateBufferWriter(buffer);
            this._codecs.Encode(message, writer); 
            output.WriteBytes(buffer);
        }
    }
}
