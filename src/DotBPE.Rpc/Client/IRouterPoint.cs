using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IRouterPoint
    {
        IPEndPoint RemoteAddress { get;  }
        RoutePointType RoutePointType { get;  }
        /// <summary>
        /// 权重
        /// </summary>
        int Weight { get;  }
        int FailCount { get; set; }
        bool Active { get; set; }
    }

    public enum RoutePointType
    {
        Local,
        Remote
    }
}
