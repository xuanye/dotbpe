// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace DotBPE.Rpc.Core
{

    public class RpcResult
    {
        public int Code {
            get; set;
        }
    }

    public class RpcResult<T> : RpcResult where T : class
    {
        public T? Data {
            get; set;
        }
    }
}
