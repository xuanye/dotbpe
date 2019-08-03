using Peach.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc.Config
{
    public class RpcServerOptions: TcpHostOption
    {
        public RpcServerOptions()
        {
            this.StartupWords = "Tomato Server bind at {0}\r\n";
            this.AppName = "Tomato";
        }
    }
}
