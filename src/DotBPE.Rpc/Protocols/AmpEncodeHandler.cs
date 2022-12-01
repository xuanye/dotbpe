// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocols
{
    public class AmpEncodeHandler : DotNetty.Codecs.MessageToByteEncoder<AmpMessage>
    {
        private readonly ISerializer _serializer;

        public AmpEncodeHandler(ISerializer serializer)
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
