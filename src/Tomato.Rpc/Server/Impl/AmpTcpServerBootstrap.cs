using Tomato.Rpc.Config;
using Tomato.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peach;
using Peach.Protocol;
using Peach.Tcp;

namespace Tomato.Rpc.Server
{
    public class AmpTcpServerBootstrap:TcpServerBootstrap<AmpMessage>
    {
        public AmpTcpServerBootstrap(
            ISocketService<AmpMessage> socketService,
            IChannelHandlerPipeline handlerPipeline,
            ILoggerFactory loggerFactory,
            IOptions<RpcServerOptions> hostOption = null)
            : base(socketService, handlerPipeline, loggerFactory, hostOption)
        {
        }
    }
}
