// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;

namespace DotBPE.Rpc.Protocols
{
    /// <summary>
    /// Decoder handler for AmpMessage
    /// </summary>
    /// <remarks>
    /// This type is obsolete. Use <see cref="DotBPE.Protocol.AmpDecodeHandler"/> instead.
    /// </remarks>
    [System.Obsolete("Use DotBPE.Protocol.AmpDecodeHandler instead")]
    public class AmpDecodeHandler : DotBPE.Protocol.AmpDecodeHandler
    {
        /// <summary>
        /// Creates a new AmpDecodeHandler
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        public AmpDecodeHandler(DotBPE.Abstractions.ISerializer serializer) 
            : base(serializer)
        {
        }
    }
}
