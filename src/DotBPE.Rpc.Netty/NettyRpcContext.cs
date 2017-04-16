using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace DotBPE.Rpc.Netty
{
    public class NettyRpcContext<TMessage> : IRpcContext<TMessage> where TMessage : IMessage
    {
        private readonly IChannel _channel;
        private readonly IMessageCodecs<TMessage> _codecs;

        public NettyRpcContext(IChannel channel, IMessageCodecs<TMessage> codecs)
        {
            this._channel = channel;
            this._codecs = codecs;
        }

        public Task CloseAsync()
        {
            return this._channel.CloseAsync();
        }

        public Task SendAsync(TMessage message)
        {
            IByteBuffer buf = GetBuffer(message);
            return this._channel.WriteAndFlushAsync(buf);
        }
        private IByteBuffer GetBuffer(TMessage message)
        {
            var buff = this._channel.Allocator.Buffer(message.Length);
            IBufferWriter writer = NettyBufferManager.CreateBufferWriter(buff);
            this._codecs.Encode(message, writer);
            return buff;
        }
    }
}
