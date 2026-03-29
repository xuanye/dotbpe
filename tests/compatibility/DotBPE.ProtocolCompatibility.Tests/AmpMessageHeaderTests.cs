// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Protocol;
using DotBPE.Protocol.Codec;
using DotNetty.Buffers;
using Xunit;

namespace DotBPE.ProtocolCompatibility.Tests
{
    /// <summary>
    /// Tests to verify AmpMessage header encoding produces expected byte layout
    /// </summary>
    public class AmpMessageHeaderTests
    {
        [Fact]
        public void AmpMessage_Version0_HeadLength_Is18()
        {
            // Arrange & Act
            var headLength = AmpMessage.VERSION_0_HEAD_LENGTH;

            // Assert
            Assert.Equal(18, headLength);
        }

        [Fact]
        public void AmpMessage_Version1_HeadLength_Is21()
        {
            // Arrange & Act
            var headLength = AmpMessage.VERSION_1_HEAD_LENGTH;

            // Assert
            Assert.Equal(21, headLength);
        }

        [Fact]
        public void AmpMessage_Version1_EncodesCorrectHeaderLayout()
        {
            // Arrange
            var message = new AmpMessage
            {
                Version = 1,
                Sequence = 12345,
                MessageType = Abstractions.Protocols.RpcMessageType.Request,
                ServiceId = 100,
                MessageId = 200,
                Code = 0,
                CodecType = CodecType.Protobuf,
                Data = new byte[] { 0x01, 0x02, 0x03 }
            };

            var buffer = Unpooled.Buffer(message.Length);

            // Act
            AmpProtocol.Encode(message, CodecType.Protobuf, buffer);

            // Assert - Verify header byte layout
            Assert.Equal(24, buffer.ReadableBytes); // 21 header + 3 data

            // Byte 0: Version
            Assert.Equal(1, buffer.GetByte(0));

            // Bytes 1-4: Length (big-endian)
            Assert.Equal(0, buffer.GetByte(1));
            Assert.Equal(0, buffer.GetByte(2));
            Assert.Equal(0, buffer.GetByte(3));
            Assert.Equal(24, buffer.GetByte(4));

            // Bytes 5-8: Sequence (big-endian)
            Assert.Equal(0, buffer.GetByte(5));
            Assert.Equal(0, buffer.GetByte(6));
            Assert.Equal(0x30, buffer.GetByte(7)); // 12345 = 0x3039
            Assert.Equal(0x39, buffer.GetByte(8));

            // Byte 9: MessageType (Request = 1)
            Assert.Equal(1, buffer.GetByte(9));

            // Bytes 10-13: ServiceId (big-endian)
            Assert.Equal(0, buffer.GetByte(10));
            Assert.Equal(0, buffer.GetByte(11));
            Assert.Equal(0, buffer.GetByte(12));
            Assert.Equal(100, buffer.GetByte(13));

            // Bytes 14-15: MessageId (big-endian, unsigned short)
            Assert.Equal(0, buffer.GetByte(14));
            Assert.Equal(200, buffer.GetByte(15));

            // Bytes 16-19: Code (big-endian)
            Assert.Equal(0, buffer.GetByte(16));
            Assert.Equal(0, buffer.GetByte(17));
            Assert.Equal(0, buffer.GetByte(18));
            Assert.Equal(0, buffer.GetByte(19));

            // Byte 20: CodecType
            Assert.Equal(0, buffer.GetByte(20)); // Protobuf = 0

            // Bytes 21-23: Data
            Assert.Equal(0x01, buffer.GetByte(21));
            Assert.Equal(0x02, buffer.GetByte(22));
            Assert.Equal(0x03, buffer.GetByte(23));
        }

        [Fact]
        public void AmpMessage_Version0_EncodesCorrectHeaderLayout()
        {
            // Arrange
            var message = new AmpMessage
            {
                Version = 0,
                Sequence = 100,
                MessageType = Abstractions.Protocols.RpcMessageType.Response,
                ServiceId = 50,
                MessageId = 10,
                Code = 0,
                CodecType = CodecType.Protobuf,
                Data = new byte[] { 0xAA, 0xBB }
            };

            var buffer = Unpooled.Buffer(message.Length);

            // Act
            AmpProtocol.Encode(message, CodecType.Protobuf, buffer);

            // Assert - Verify header byte layout
            Assert.Equal(20, buffer.ReadableBytes); // 18 header + 2 data

            // Byte 0: Version
            Assert.Equal(0, buffer.GetByte(0));

            // Bytes 1-4: Length (big-endian)
            Assert.Equal(0, buffer.GetByte(1));
            Assert.Equal(0, buffer.GetByte(2));
            Assert.Equal(0, buffer.GetByte(3));
            Assert.Equal(20, buffer.GetByte(4));

            // Bytes 5-8: Sequence (big-endian)
            Assert.Equal(0, buffer.GetByte(5));
            Assert.Equal(0, buffer.GetByte(6));
            Assert.Equal(0, buffer.GetByte(7));
            Assert.Equal(100, buffer.GetByte(8));

            // Byte 9: MessageType (Response = 2)
            Assert.Equal(2, buffer.GetByte(9));

            // Bytes 10-11: ServiceId (unsigned short for version 0)
            Assert.Equal(0, buffer.GetByte(10));
            Assert.Equal(50, buffer.GetByte(11));

            // Bytes 12-13: MessageId (big-endian, unsigned short)
            Assert.Equal(0, buffer.GetByte(12));
            Assert.Equal(10, buffer.GetByte(13));

            // Bytes 14-17: Code (big-endian)
            Assert.Equal(0, buffer.GetByte(14));
            Assert.Equal(0, buffer.GetByte(15));
            Assert.Equal(0, buffer.GetByte(16));
            Assert.Equal(0, buffer.GetByte(17));

            // Bytes 18-19: Data
            Assert.Equal(0xAA, buffer.GetByte(18));
            Assert.Equal(0xBB, buffer.GetByte(19));
        }

        [Fact]
        public void AmpMessage_HeartBeat_HasZeroServiceIdAndMessageId()
        {
            // Arrange
            var message = AmpMessage.CreateHeartBeatMessage(CodecType.Protobuf);

            // Act & Assert
            Assert.True(message.IsHeartBeat);
            Assert.Equal(0, message.ServiceId);
            Assert.Equal(0, message.MessageId);
        }
    }
}
