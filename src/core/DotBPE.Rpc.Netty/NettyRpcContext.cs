using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace DotBPE.Rpc.Netty {
    /// <summary>
    /// 这个类是实现多连接的关键类，由这个类来实现多连接的切换
    /// </summary>
    public class NettyRpcContext<TMessage> : IRpcContext<TMessage> where TMessage : InvokeMessage {
        private readonly IChannel _channel;
        private readonly IMessageCodecs<TMessage> _codecs;

        public NettyRpcContext (IChannel channel, IMessageCodecs<TMessage> codecs) {
            this._channel = channel;
            this._codecs = codecs;
        }

        public EndPoint RemoteAddress { get; set; }

        public EndPoint LocalAddress { get; set; }

        public Task CloseAsync () {
            return this._channel.CloseAsync ();
        }

        public Task SendAsync (TMessage message) {
          
            if (this._channel.IsWritable)
            {
                IByteBuffer buf = GetBuffer(message);
                return this._channel.WriteAndFlushAsync(buf);
            }
            return Task.CompletedTask;
        }

        private IByteBuffer GetBuffer (TMessage message) {
            var buff = this._channel.Allocator.Buffer (message.Length);
            IBufferWriter writer = NettyBufferManager.CreateBufferWriter (buff);
            this._codecs.Encode (message, writer);
            return buff;
        }
    }
}
