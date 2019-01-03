using Peach.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Config
{
    public class RpcServerOptions: TcpHostOption
    {
        public RpcServerOptions()
        {
            this.StartupWords = "DotBPE Server bind at {0}";
        }

    }
}
