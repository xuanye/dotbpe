// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Rpc.Codec
{
    /// <summary>
    /// Codec type enumeration
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Protocol.Codec.CodecType"/> instead.
    /// </remarks>
    [Obsolete("Use DotBPE.Protocol.Codec.CodecType instead")]
    public enum CodecType : byte
    {
        /// <summary>
        /// Protobuf serialization (default)
        /// </summary>
        Protobuf = 0,
        /// <summary>
        /// MessagePack serialization
        /// </summary>
        MessagePack = 1,
        /// <summary>
        /// JSON serialization
        /// </summary>
        JSON = 2,
        /// <summary>
        /// Unknown codec type
        /// </summary>
        Unknown = 9,
    }
}
