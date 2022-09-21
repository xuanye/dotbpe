using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Options;
using Peach;
using Peach.Tcp;

namespace DotBPE.Rpc.Client
{
    public class RpcSocketClient : TcpClient<AmpMessage>
    {
        public RpcSocketClient(IOptions<RpcClientOptions> clientOption, IChannelHandlerPipeline handlerPipeline)
            : base(clientOption, handlerPipeline)
        {

        }
       
    }
}
