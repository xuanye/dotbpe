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
            var timeOut = isServer ? AmpProtocol.HeartbeatInterval * 2 : AmpProtocol.HeartbeatInterval;
            return new Dictionary<string, IChannelHandler> {

                { "timeout", new IdleStateHandler(0, 0,timeOut) },

                { "message-enc", new AmpEncodeHandler(_serializer) },
                { "framing-dec", new LengthFieldBasedFrameDecoder(
                    AmpProtocol.MaxFrameLength,
                    AmpProtocol.LengthFieldOffset,
                    AmpProtocol.LengthFieldLength,
                    AmpProtocol.LengthAdjustment,
                    AmpProtocol.InitialBytesToStrip
                ) },
                { "message-dec", new AmpDecodeHandler(_serializer) }
            };
        }
    }
}
