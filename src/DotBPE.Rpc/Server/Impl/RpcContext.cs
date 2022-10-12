// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public class RpcContext : IRpcContext
    {
        public RpcContext(IPEndPoint remoteAddress, IPEndPoint localAddress)
        {
            RemoteAddress = remoteAddress;
            LocalAddress = localAddress;
        }

        public IPEndPoint RemoteAddress { get; set; }

        public IPEndPoint LocalAddress { get; set; }
    }

    public class LocalRpcContext : IRpcContext
    {

        public static readonly LocalRpcContext Instance = new LocalRpcContext();

        private static readonly IPEndPoint _loopbackPoint = new IPEndPoint(IPAddress.Loopback, 0);

        public IPEndPoint RemoteAddress => _loopbackPoint;

        public IPEndPoint LocalAddress => _loopbackPoint;
    }
}
