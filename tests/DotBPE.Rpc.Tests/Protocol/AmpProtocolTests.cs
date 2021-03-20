using System.IO;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocol;
using Moq;
using Xunit;

namespace DotBPE.Rpc.Tests.Protocol
{
    public class AmpProtocolTests
    {
        [Fact]
        public void PackAndUnPackTest()
        {
            /*
            var src = AmpMessage.CreateRequestMessage(1, 1);
            src.Version = 1;
            src.Code = 100;
            src.Sequence = 11;
            src.CodecType = CodecType.MessagePack;
            src.Data = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};

            var serializer = new Mock<ISerializer>();
            serializer.SetupGet(x => x.CodecType).Returns((byte) CodecType.MessagePack);

            var protocol = new AmpProtocol(serializer.Object);

            var stream = new MemoryStream();

            var writer = new MemoryBufferWriter(stream);

            protocol.Pack(writer, src);

            Assert.Equal(AmpMessage.VERSION_1_HEAD_LENGTH + 10, stream.Length);

            var reader = new MemoryBufferReader(stream);

            var dist = protocol.Parse(reader);

            Assert.NotNull(dist);

            Assert.Equal(src.Id, dist.Id);
            Assert.Equal(src.Version, dist.Version);
            Assert.Equal(src.Code, dist.Code);
            Assert.Equal(src.Sequence, dist.Sequence);
            Assert.Equal(src.CodecType, dist.CodecType);
            Assert.Equal(src.Data.Length, dist.Data.Length);
            */
        }
    }
}
