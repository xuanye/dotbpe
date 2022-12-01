// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Peach;
using System.Collections.Generic;

namespace DotBPE.Rpc.Protocols
{
    public class AmpChannelHandlerPipeline : IChannelHandlerPipeline
    {
        private readonly ISerializer _serializer;

        public AmpChannelHandlerPipeline(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {
            var timeOut = isServer ? AmpProtocol.HEARTBEAT_INTERVAL * 2 : AmpProtocol.HEARTBEAT_INTERVAL;
            return new Dictionary<string, IChannelHandler> {

                { "timeout", new IdleStateHandler(0, 0,timeOut) },

                { "message-enc", new AmpEncodeHandler(_serializer) },
                { "framing-dec", new LengthFieldBasedFrameDecoder(
                    AmpProtocol.MAX_FRAME_LENGTH,
                    AmpProtocol.LENGTH_FIELD_OFFSET,
                    AmpProtocol.LENGTH_FIELD_LENGTH,
                    AmpProtocol.LENGTH_ADJUSTMENT,
                    AmpProtocol.INITIAL_BYTES_TO_STRIP
                ) },
                { "message-dec", new AmpDecodeHandler(_serializer) }
            };
        }
    }
}
