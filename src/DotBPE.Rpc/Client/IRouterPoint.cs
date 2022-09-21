// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Net;

namespace DotBPE.Rpc.Client
{
    public interface IRouterPoint
    {
        IPEndPoint RemoteAddress { get; }
        RoutePointType RoutePointType { get; }
        int Weight { get; }
        int FailCount { get; set; }
        bool Active { get; set; }
    }

    public class RouterPoint : IRouterPoint
    {
        public IPEndPoint RemoteAddress { get; set; }
        public RoutePointType RoutePointType { get; set; }
        public int FailCount { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool Active { get; set; } = true;

        public int Weight { get; set; } = 1;
    }

    public enum RoutePointType
    {
        Local,
        Remote
    }
}
