using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Peach.Buffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Tcp
{
    /// <summary>
    /// 消息解码器
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ChannelDecodeHandler<TMessage> : DotNetty.Codecs.ByteToMessageDecoder where TMessage : Messaging.IMessage
    {
        private readonly Protocol.IProtocol<TMessage> _protocol;

        public ChannelDecodeHandler(Protocol.IProtocol<TMessage> protocol)
        {
            this._protocol = protocol;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            IBufferReader reader = ByteBufferManager.CreateBufferReader(input);
                       
            var message = this._protocol.Parse(reader);
            if (message != null)
            {
                output.Add(message);
            }
            reader = null;
        }
    }
}
