using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peach;
using Peach.Protocol;
using Peach.Tcp;

namespace DotBPE.Rpc.Server
{
    public class AmpTcpServerBootstrap:TcpServerBootstrap<AmpMessage>
    {
        public AmpTcpServerBootstrap(
            ISocketService<AmpMessage> socketService,
            IProtocol<AmpMessage> protocol,
            ILoggerFactory loggerFactory,
            IOptions<RpcServerOptions> hostOption = null)
            : base(socketService, protocol, loggerFactory, hostOption)
        {
        }
    }
}
