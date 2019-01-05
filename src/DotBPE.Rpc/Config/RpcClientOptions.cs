using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Config
{
    public class RpcClientOptions:Peach.Config.TcpClientOption
    {
        public string AppName { get; set; } = "dotbpe";
    }
}
