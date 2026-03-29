// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Protocol.Codec;
using Xunit;

namespace DotBPE.ProtocolCompatibility.Tests
{
    /// <summary>
    /// Tests to verify CodecType enum values remain stable for binary protocol compatibility
    /// </summary>
    public class CodecTypeStabilityTests
    {
        [Fact]
        public void CodecType_Protobuf_HasValueZero()
        {
            // Arrange & Act
            var value = (byte)CodecType.Protobuf;

            // Assert
            Assert.Equal(0, value);
        }

        [Fact]
        public void CodecType_MessagePack_HasValueOne()
        {
            // Arrange & Act
            var value = (byte)CodecType.MessagePack;

            // Assert
            Assert.Equal(1, value);
        }

        [Fact]
        public void CodecType_JSON_HasValueTwo()
        {
            // Arrange & Act
            var value = (byte)CodecType.JSON;

            // Assert
            Assert.Equal(2, value);
        }

        [Fact]
        public void CodecType_Unknown_HasValueNine()
        {
            // Arrange & Act
            var value = (byte)CodecType.Unknown;

            // Assert
            Assert.Equal(9, value);
        }
    }
}
