using System.Net;

namespace DotBPE.Rpc {
    public class RouterPoint {
        public EndPoint RemoteAddress { get; set; }

        public RoutePointType RoutePointType { get; set; }
    }

    public enum RoutePointType {
        Local,
        Remote
    }
}