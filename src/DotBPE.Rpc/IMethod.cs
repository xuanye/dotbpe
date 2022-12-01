// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IMethod
    {
        string GroupName { get; }
        string ServiceName { get; }
        string MethodName { get; }
        int ServiceId { get; }
        ushort MethodId { get; }
        string FullName { get; }

        int DefaultTimeout { get; set; }
    }
}
