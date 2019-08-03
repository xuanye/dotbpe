using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Peach;

namespace Tomato.Rpc.Protocol
{
    public class AmpChannelHandlerPipeline:IChannelHandlerPipeline
    {
        private readonly ISerializer _serializer;

        public AmpChannelHandlerPipeline(ISerializer serializer)
        {
            this._serializer = serializer;
        }
        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {

            /*
             *
             *    pipeline.AddLast("timeout", new IdleStateHandler(0, 0, meta.HeartbeatInterval / 1000 * 2)); //服务端双倍来处理

                        //消息前处理
                        pipeline.AddLast(
                            new LengthFieldBasedFrameDecoder(
                                meta.MaxFrameLength,
                                meta.LengthFieldOffset,
                                meta.LengthFieldLength,
                                meta.LengthAdjustment,
                                meta.InitialBytesToStrip
                            )
                        );
             */
            return new Dictionary<string, IChannelHandler> {

                { "timeout", new IdleStateHandler(0, 0, AmpProtocol.HeartbeatInterval/1000*2) }, //服务端双倍来处理

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
