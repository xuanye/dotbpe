using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using DotNetty.Buffers;
using Xunit;

namespace DotBPE.Test.Amp
{
    public class AmpCodecsTest
    {
        [Fact]
        public void TestEncodeAndDecode()
        {
            var codecs = new AmpCodecs();
            var msg = new AmpMessage
            {
                InvokeMessageType = InvokeMessageType.Request,
                ServiceId = 10000,
                MessageId = 101
            };
            msg.Data = Encoding.UTF8.GetBytes(msg.UniqueId);
            IByteBuffer buffer = Unpooled.Buffer(msg.Length);

            var bufferReader =new NettyByteBufferReader(buffer);
            var bufferWriter = new NettyByteBufferWriter(buffer);
            codecs.Encode(msg, bufferWriter);

            var otherMsg = codecs.Decode(bufferReader);

            Assert.Equal(msg.MessageId, otherMsg.MessageId);
            Assert.Equal(msg.ServiceId, otherMsg.ServiceId);
            string uid = Encoding.UTF8.GetString(otherMsg.Data);
            Assert.Equal(msg.UniqueId, uid);
        }
    }
}
