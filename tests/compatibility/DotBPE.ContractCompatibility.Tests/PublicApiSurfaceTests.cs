// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Protocol;
using DotBPE.Protocol.Codec;
using Xunit;

namespace DotBPE.ContractCompatibility.Tests
{
    /// <summary>
    /// Contract compatibility tests verify public API surface hasn't changed
    /// These tests ensure backward compatibility of the public contract
    /// </summary>
    public class PublicApiSurfaceTests
    {
        [Fact]
        public void ISerializer_Interface_HasExpectedMethods()
        {
            // Arrange
            var interfaceType = typeof(ISerializer);

            // Act & Assert - Verify interface has required methods
            Assert.NotNull(interfaceType.GetMethod("Serialize", new[] { typeof(object) }));
            Assert.NotNull(interfaceType.GetMethod("Deserialize", new[] { typeof(byte[]), typeof(System.Type) }));
            Assert.NotNull(interfaceType.GetProperty("CodecType"));
        }

        [Fact]
        public void AmpMessage_Class_HasExpectedProperties()
        {
            // Arrange
            var messageType = typeof(AmpMessage);

            // Act & Assert - Verify message class has required properties
            Assert.NotNull(messageType.GetProperty("Version"));
            Assert.NotNull(messageType.GetProperty("Sequence"));
            Assert.NotNull(messageType.GetProperty("MessageType"));
            Assert.NotNull(messageType.GetProperty("ServiceId"));
            Assert.NotNull(messageType.GetProperty("MessageId"));
            Assert.NotNull(messageType.GetProperty("Code"));
            Assert.NotNull(messageType.GetProperty("CodecType"));
            Assert.NotNull(messageType.GetProperty("Data"));
        }

        [Fact]
        public void CodecType_Enum_HasExpectedValues()
        {
            // Arrange
            var enumType = typeof(CodecType);

            // Act & Assert - Verify enum has expected values
            Assert.NotNull(enumType.GetField("Protobuf"));
            Assert.NotNull(enumType.GetField("MessagePack"));
            Assert.NotNull(enumType.GetField("JSON"));
            Assert.NotNull(enumType.GetField("Unknown"));
        }

        [Fact]
        public void AmpProtocol_Class_HasExpectedMethods()
        {
            // Arrange
            var protocolType = typeof(AmpProtocol);

            // Act & Assert - Verify protocol class has required methods
            Assert.NotNull(protocolType.GetMethod("Encode"));
            Assert.NotNull(protocolType.GetMethod("Decode"));
        }
    }
}
