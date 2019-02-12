using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public interface IRpcContext
    {
        IPEndPoint RemoteAddress { get; }
        IPEndPoint LocalAddress { get; }
    }
}
