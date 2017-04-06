using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Netty
{
    public class NettyServerMessageSender<TMessage> : IMessageSender<TMessage> where TMessage : IMessage
    {
        private readonly IChannelHandlerContext _context;
        private readonly IMessageCodecs<TMessage> _codecs;
        private readonly NettyBufferManager bufferManager;
        public NettyServerMessageSender(IChannelHandlerContext context,IMessageCodecs<TMessage> codecs)
        {
            this._context = context;
            this._codecs = codecs;
            this.bufferManager = new NettyBufferManager();
        }

        public Task SendAndFlushAsync(TMessage message)
        {
            IByteBuffer buf = GetBuffer(message);
            return this._context.WriteAndFlushAsync(buf);
        }
        private IByteBuffer GetBuffer(TMessage message)
        {
            var buff = this._context.Allocator.Buffer(message.Length);
            IBufferWriter writer = this.bufferManager.CreateBufferWriter(buff);
            this._codecs.Encode(message, writer);
            return buff;
        }

        public Task SendAsync(TMessage message)
        {
            IByteBuffer buf = GetBuffer(message);
            return this._context.WriteAsync(buf);
        }
    }
}
