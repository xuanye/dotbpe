// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Extensions;
using Xunit;

namespace DotBPE.SerializerCompatibility.Tests
{
    /// <summary>
    /// Compatibility tests for Newtonsoft.Json serializer
    /// Verifies serialization produces consistent output and deserialization restores original data
    /// </summary>
    public class JsonNetSerializerCompatibilityTests
    {
        private readonly ISerializer _serializer;

        public JsonNetSerializerCompatibilityTests()
        {
            _serializer = new JsonSerializer();
        }

        [Fact]
        public void JsonNet_CodecType_IsTwo()
        {
            // Arrange & Act
            var codecType = _serializer.CodecType;

            // Assert - JsonNet uses same codec type as System.Text.Json
            Assert.Equal(2, codecType);
        }

        [Fact]
        public void JsonNet_RoundTrip_PreservesSimpleMessage()
        {
            // Arrange
            var original = new TestMessage
            {
                Id = 42,
                Name = "Test",
                IsActive = true,
                Score = 99.5
            };

            // Act
            var bytes = _serializer.Serialize(original);
            var deserialized = _serializer.Deserialize<TestMessage>(bytes);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Id, deserialized.Id);
            Assert.Equal(original.Name, deserialized.Name);
            Assert.Equal(original.IsActive, deserialized.IsActive);
            Assert.Equal(original.Score, deserialized.Score);
        }

        [Fact]
        public void JsonNet_RoundTrip_PreservesComplexMessage()
        {
            // Arrange
            var original = new ComplexTestMessage
            {
                Id = 100,
                Inner = new TestMessage
                {
                    Id = 1,
                    Name = "Inner",
                    IsActive = false,
                    Score = 50.0
                },
                Values = new[] { 1, 2, 3, 4, 5 }
            };

            // Act
            var bytes = _serializer.Serialize(original);
            var deserialized = _serializer.Deserialize<ComplexTestMessage>(bytes);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Id, deserialized.Id);
            Assert.NotNull(deserialized.Inner);
            Assert.Equal(original.Inner.Id, deserialized.Inner.Id);
            Assert.Equal(original.Inner.Name, deserialized.Inner.Name);
            Assert.NotNull(deserialized.Values);
            Assert.Equal(original.Values.Length, deserialized.Values.Length);
        }

        [Fact]
        public void JsonNet_Serialize_ReturnsNonNullBytes()
        {
            // Arrange
            var message = new TestMessage { Id = 1, Name = "Test" };

            // Act
            var bytes = _serializer.Serialize(message);

            // Assert
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0);
        }

        [Fact]
        public void JsonNet_Deserialize_WithNull_ReturnsDefault()
        {
            // Act
            var result = _serializer.Deserialize<TestMessage>(null);

            // Assert
            Assert.Null(result);
        }
    }
}
