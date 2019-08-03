using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tomato.Rpc.Server
{
    public interface IRpcContext
    {
        IPEndPoint RemoteAddress { get; }
        IPEndPoint LocalAddress { get; }
    }
}
