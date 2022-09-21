// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Hosting
{
    public class RpcServerOptions : TcpHostOption
    {
        public RpcServerOptions()
        {
            StartupWords = "DotBPE Server bind at {0}";
            AppName = "dotbpe";
        }
    }
}
