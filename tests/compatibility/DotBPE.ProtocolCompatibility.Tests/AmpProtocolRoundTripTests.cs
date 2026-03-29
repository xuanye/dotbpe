// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Protocol;
using DotBPE.Protocol.Codec;
using DotNetty.Buffers;
using System;
using Xunit;

namespace DotBPE.ProtocolCompatibility.Tests
{
    /// <summary>
    /// Tests to verify round-trip encode/decode produces identical message
    /// </summary>
    public class AmpProtocolRoundTripTests
    {
        [Fact]
        public void RoundTrip_Version1_PreservesAllFields()
        {
            // Arrange
            var original = new AmpMessage
            {
                Version = 1,
                Sequence = 99999,
                MessageType = Abstractions.Protocols.RpcMessageType.Request,
                ServiceId = 12345,
                MessageId = 6789,
                Code = 0,
                CodecType = CodecType.MessagePack,
                Data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }
            };

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.MessagePack, buffer);
            var decoded = AmpProtocol.Decode(CodecType.MessagePack, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(original.Version, decoded.Version);
            Assert.Equal(original.Sequence, decoded.Sequence);
            Assert.Equal(original.MessageType, decoded.MessageType);
            Assert.Equal(original.ServiceId, decoded.ServiceId);
            Assert.Equal(original.MessageId, decoded.MessageId);
            Assert.Equal(original.Code, decoded.Code);
            Assert.Equal(original.CodecType, decoded.CodecType);
            Assert.Equal(original.Data, decoded.Data);
        }

        [Fact]
        public void RoundTrip_Version0_PreservesAllFields()
        {
            // Arrange
            var original = new AmpMessage
            {
                Version = 0,
                Sequence = 1234,
                MessageType = Abstractions.Protocols.RpcMessageType.Response,
                ServiceId = 100,
                MessageId = 200,
                Code = 0,
                CodecType = CodecType.Protobuf,
                Data = new byte[] { 0xAA, 0xBB, 0xCC }
            };

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.Protobuf, buffer);
            var decoded = AmpProtocol.Decode(CodecType.Protobuf, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(original.Version, decoded.Version);
            Assert.Equal(original.Sequence, decoded.Sequence);
            Assert.Equal(original.MessageType, decoded.MessageType);
            Assert.Equal(original.ServiceId, decoded.ServiceId);
            Assert.Equal(original.MessageId, decoded.MessageId);
            Assert.Equal(original.Code, decoded.Code);
            Assert.Equal(CodecType.Protobuf, decoded.CodecType); // Version 0 always uses Protobuf
            Assert.Equal(original.Data, decoded.Data);
        }

        [Fact]
        public void RoundTrip_EmptyData_PreservesAllFields()
        {
            // Arrange
            var original = new AmpMessage
            {
                Version = 1,
                Sequence = 1,
                MessageType = Abstractions.Protocols.RpcMessageType.Request,
                ServiceId = 1,
                MessageId = 1,
                Code = 0,
                CodecType = CodecType.JSON,
                Data = null
            };

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.JSON, buffer);
            var decoded = AmpProtocol.Decode(CodecType.JSON, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(original.Version, decoded.Version);
            Assert.Equal(original.Sequence, decoded.Sequence);
            Assert.Equal(original.MessageType, decoded.MessageType);
            Assert.Equal(original.ServiceId, decoded.ServiceId);
            Assert.Equal(original.MessageId, decoded.MessageId);
            Assert.Equal(original.Code, decoded.Code);
            Assert.Equal(original.CodecType, decoded.CodecType);
            Assert.Null(decoded.Data);
        }

        [Fact]
        public void RoundTrip_LargeData_PreservesAllFields()
        {
            // Arrange
            var random = new Random(42); // Fixed seed for reproducibility
            var largeData = new byte[1024];
            random.NextBytes(largeData);

            var original = new AmpMessage
            {
                Version = 1,
                Sequence = int.MaxValue,
                MessageType = Abstractions.Protocols.RpcMessageType.Response,
                ServiceId = int.MaxValue,
                MessageId = ushort.MaxValue,
                Code = 404,
                CodecType = CodecType.Protobuf,
                Data = largeData
            };

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.Protobuf, buffer);
            var decoded = AmpProtocol.Decode(CodecType.Protobuf, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(original.Version, decoded.Version);
            Assert.Equal(original.Sequence, decoded.Sequence);
            Assert.Equal(original.MessageType, decoded.MessageType);
            Assert.Equal(original.ServiceId, decoded.ServiceId);
            Assert.Equal(original.MessageId, decoded.MessageId);
            Assert.Equal(original.Code, decoded.Code);
            Assert.Equal(original.CodecType, decoded.CodecType);
            Assert.Equal(original.Data, decoded.Data);
        }

        [Fact]
        public void RoundTrip_HeartBeatMessage_WorksCorrectly()
        {
            // Arrange
            var original = AmpMessage.CreateHeartBeatMessage(CodecType.Protobuf);

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.Protobuf, buffer);
            var decoded = AmpProtocol.Decode(CodecType.Protobuf, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.True(decoded.IsHeartBeat);
            Assert.Equal(0, decoded.ServiceId);
            Assert.Equal(0, decoded.MessageId);
        }

        [Fact]
        public void RoundTrip_RequestMessage_FactoryMethod()
        {
            // Arrange
            var original = AmpMessage.CreateRequestMessage(100, 200, CodecType.MessagePack);

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.MessagePack, buffer);
            var decoded = AmpProtocol.Decode(CodecType.MessagePack, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(100, decoded.ServiceId);
            Assert.Equal(200, decoded.MessageId);
            Assert.Equal(Abstractions.Protocols.RpcMessageType.Request, decoded.MessageType);
            Assert.Equal(CodecType.MessagePack, decoded.CodecType);
        }

        [Fact]
        public void RoundTrip_ResponseMessage_FactoryMethod()
        {
            // Arrange
            var request = AmpMessage.CreateRequestMessage(100, 200, CodecType.JSON);
            request.Sequence = 42;
            var original = AmpMessage.CreateResponseMessage(request);

            var buffer = Unpooled.Buffer(original.Length);

            // Act
            AmpProtocol.Encode(original, CodecType.JSON, buffer);
            var decoded = AmpProtocol.Decode(CodecType.JSON, buffer);

            // Assert
            Assert.NotNull(decoded);
            Assert.Equal(100, decoded.ServiceId);
            Assert.Equal(200, decoded.MessageId);
            Assert.Equal(42, decoded.Sequence);
            Assert.Equal(Abstractions.Protocols.RpcMessageType.Response, decoded.MessageType);
            Assert.Equal(CodecType.JSON, decoded.CodecType);
        }
    }
}
