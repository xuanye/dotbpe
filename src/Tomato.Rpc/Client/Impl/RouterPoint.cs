using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tomato.Rpc.Client
{
    public class RouterPoint : IRouterPoint
    {
        public IPEndPoint RemoteAddress { get; set; }
        public RoutePointType RoutePointType { get; set; }
        public int FailCount { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool Active { get; set; } = true;

        public int Weight { get; set; } = 1;
    }
}
