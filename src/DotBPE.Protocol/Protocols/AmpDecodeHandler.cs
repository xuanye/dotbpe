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
    /// Decoder handler for AmpMessage
    /// </summary>
    public class AmpDecodeHandler : DotNetty.Codecs.ByteToMessageDecoder
    {
        private readonly Abstractions.ISerializer _serializer;

        /// <summary>
        /// Creates a new AmpDecodeHandler
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        public AmpDecodeHandler(Abstractions.ISerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var msg = AmpProtocol.Decode((CodecType)_serializer.CodecType, input);
            if (msg == null)
            {
                return;
            }
            output.Add(msg);
        }
    }
}
