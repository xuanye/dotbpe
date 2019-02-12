using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public class RpcContext : IRpcContext
    {
        public IPEndPoint RemoteAddress { get; set; }

        public IPEndPoint LocalAddress { get; set; }
    }

    public class LocalRpcContext : IRpcContext
    {

        public static LocalRpcContext Local = new LocalRpcContext();

        private static readonly IPEndPoint LOCAL_POINT = new IPEndPoint(IPAddress.Loopback, 0);
        public IPEndPoint RemoteAddress {
            get
            {
                return LOCAL_POINT;
            }
        }

        public IPEndPoint LocalAddress
        {
            get
            {
                return LOCAL_POINT;
            }
        }
    }
}
