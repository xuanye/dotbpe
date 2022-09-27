using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IRoutingPolicy
    {
        IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses);
    }
}
