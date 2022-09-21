// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Rpc
{

    public class Method : IMethod
    {
        public string? GroupName { get; set; }
        public string? ServiceName { get; set; }
        public string? MethodName { get; set; }

        public int ServiceId { get; set; }
        public ushort MethodId { get; set; }
        public int DefaultTimeout { get; set; }
        public string FullName => $"{ServiceName}.{MethodName}";
        public MethodInfo Handler { get; set; }
    }
}
