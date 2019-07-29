using Tomato.Rpc.Config;
using Tomato.Rpc.Protocol;
using Microsoft.Extensions.Options;
using Peach.Config;
using Peach.Protocol;
using Peach.Tcp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc.Client
{
    public class RpcSocketClient : TcpClient<AmpMessage>
    {
        public RpcSocketClient(IOptions<RpcClientOptions> clientOption, IProtocol<AmpMessage> protocol)
            : base(clientOption, protocol)
        {
        }
       
    }
}
