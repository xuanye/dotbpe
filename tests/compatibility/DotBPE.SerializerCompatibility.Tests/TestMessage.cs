// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Runtime.Serialization;

namespace DotBPE.SerializerCompatibility.Tests
{
    /// <summary>
    /// Sample test message for serializer compatibility testing
    /// Uses DataContract attributes for cross-serializer compatibility
    /// </summary>
    [DataContract]
    public class TestMessage
    {
        /// <summary>
        /// Integer value field
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// String value field
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Boolean value field
        /// </summary>
        [DataMember(Order = 3)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Double value field
        /// </summary>
        [DataMember(Order = 4)]
        public double Score { get; set; }
    }

    /// <summary>
    /// Nested test message for complex serialization scenarios
    /// </summary>
    [DataContract]
    public class ComplexTestMessage
    {
        /// <summary>
        /// Simple identifier
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Nested message reference
        /// </summary>
        [DataMember(Order = 2)]
        public TestMessage Inner { get; set; }

        /// <summary>
        /// Array of integers
        /// </summary>
        [DataMember(Order = 3)]
        public int[] Values { get; set; }
    }
}
