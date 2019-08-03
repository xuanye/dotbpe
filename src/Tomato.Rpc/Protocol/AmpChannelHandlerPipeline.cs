using System.Collections.Generic;
using DotNetty.Transport.Channels;
using Peach;

namespace Tomato.Rpc.Protocol
{
    public class AmpChannelHandlerPipeline:IChannelHandlerPipeline
    {
        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {
            throw new System.NotImplementedException();
        }
    }
}
