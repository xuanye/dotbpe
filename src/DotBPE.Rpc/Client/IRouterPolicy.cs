using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IRouterPolicy
    {
        IRouterPoint Select(string serviceKey, List<IRouterPoint> remoteAddresses);
    }
}
