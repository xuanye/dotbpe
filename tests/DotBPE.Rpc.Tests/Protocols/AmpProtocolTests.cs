// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocols;
using DotNetty.Buffers;
using Moq;
using System.IO;
using Xunit;

namespace DotBPE.Rpc.Tests.Protocol
{
    public class AmpProtocolTests
    {
        [Fact]
        public void CreateRequestMessageWithRespsone_ShouldBe_DecodeIncorrect()
        {
            var codecType = CodecType.MessagePack;
            /**/
            var src = AmpMessage.CreateRequestMessage(1, 1, codecType, true);
            src.Version = 1;
            src.Code = 100;
            src.Sequence = 11;
            src.CodecType = codecType;
            src.Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var buffer = Unpooled.Buffer(src.Length);

            AmpProtocol.Encode(src, codecType, buffer);

            var buffer2 = Unpooled.WrappedBuffer(buffer);
            var dist = AmpProtocol.Decode(codecType, buffer2);

            Assert.NotNull(dist);

            Assert.Equal(src.Id, dist.Id);
            Assert.Equal(src.Version, dist.Version);
            Assert.Equal(src.Code, dist.Code);
            Assert.Equal(src.Sequence, dist.Sequence);
            Assert.Equal(src.CodecType, dist.CodecType);
            Assert.Equal(src.Data.Length, dist.Data.Length);

        }
    }
}
