// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Rpc.Server.Impl
{
    internal class ServerMethod : Method
    {
        public MethodInfo Handler { get; set; }
    }
}
