using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public class RouterPoint : IRouterPoint
    {
        public EndPoint RemoteAddress { get; set; }
        public RoutePointType RoutePointType { get; set; }
        public int FailCount { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool Active { get; set; } = true;

        public int Weight { get; set; } = 1;
    }
}
