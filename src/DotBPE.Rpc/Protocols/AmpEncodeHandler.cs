// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;

namespace DotBPE.Rpc.Protocols
{
    /// <summary>
    /// Encoder handler for AmpMessage
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Protocol.AmpEncodeHandler"/> instead.
    /// </remarks>
    [System.Obsolete("Use DotBPE.Protocol.AmpEncodeHandler instead")]
    public class AmpEncodeHandler : DotBPE.Protocol.AmpEncodeHandler
    {
        /// <summary>
        /// Creates a new AmpEncodeHandler
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        public AmpEncodeHandler(DotBPE.Abstractions.ISerializer serializer) 
            : base(serializer)
        {
        }
    }
}
