// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Protocol.Codec;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Abstractions = DotBPE.Abstractions;
using System.Collections.Generic;

namespace DotBPE.Protocol
{
    /// <summary>
    /// Encoder handler for AmpMessage
    /// </summary>
    public class AmpEncodeHandler : DotNetty.Codecs.MessageToByteEncoder<AmpMessage>
    {
        private readonly Abstractions.ISerializer _serializer;

        /// <summary>
        /// Creates a new AmpEncodeHandler
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        public AmpEncodeHandler(Abstractions.ISerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Encode(IChannelHandlerContext context, AmpMessage message, IByteBuffer output)
        {
            if (message == null)
            {
                return;
            }
            AmpProtocol.Encode(message, (CodecType)_serializer.CodecType, output);
        }
    }
}
