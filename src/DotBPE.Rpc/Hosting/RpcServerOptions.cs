// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Config;

namespace DotBPE.Rpc.Hosting
{
    public class RpcServerOptions : TcpHostOption
    {
        public RpcServerOptions()
        {
            StartupWords = "DotBPE Server bind at {0}\r\n";
            AppName = "DotBPE";
        }

        public static readonly RpcServerOptions Default = new RpcServerOptions()
        {
            Port = 5566,
            BindType = AddressBindType.Loopback
        };
    }
}
