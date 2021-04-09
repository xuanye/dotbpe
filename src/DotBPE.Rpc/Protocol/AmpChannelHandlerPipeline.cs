using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocol
{
    public class AmpChannelHandlerPipeline : IChannelHandlerPipeline
    {
        private readonly ISerializer _serializer;

        public AmpChannelHandlerPipeline(ISerializer serializer)
        {
            this._serializer = serializer;
        }
        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {
            int timeOut = isServer ? AmpProtocol.HeartbeatInterval * 2 : AmpProtocol.HeartbeatInterval ;
            return new Dictionary<string, IChannelHandler> {

                { "timeout", new IdleStateHandler(0, 0,timeOut) }, //服务端双倍来处理

                { "message-enc", new AmpEncodeHandler(this._serializer) },
                { "framing-dec", new LengthFieldBasedFrameDecoder(
                    AmpProtocol.MaxFrameLength,
                    AmpProtocol.LengthFieldOffset,
                    AmpProtocol.LengthFieldLength,
                    AmpProtocol.LengthAdjustment,
                    AmpProtocol.InitialBytesToStrip
                ) },
                { "message-dec", new AmpDecodeHandler(this._serializer) }
            };
        }
    }
}
