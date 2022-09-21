// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocols
{
    public class AmpDecodeHandler : DotNetty.Codecs.ByteToMessageDecoder
    {
        private readonly ISerializer _serializer;

        public AmpDecodeHandler(ISerializer serializer)
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
