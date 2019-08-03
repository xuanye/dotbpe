using System.Net;

namespace Tomato.Rpc.Server
{
    public class RpcContext : IRpcContext
    {
        public IPEndPoint RemoteAddress { get; set; }

        public IPEndPoint LocalAddress { get; set; }
    }

    public class LocalRpcContext : IRpcContext
    {

        public static readonly LocalRpcContext Local = new LocalRpcContext();

        private static readonly IPEndPoint LOCAL_POINT = new IPEndPoint(IPAddress.Loopback, 0);
        public IPEndPoint RemoteAddress => LOCAL_POINT;

        public IPEndPoint LocalAddress => LOCAL_POINT;
    }
}
