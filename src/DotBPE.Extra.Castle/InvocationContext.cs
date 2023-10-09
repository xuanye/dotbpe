// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Extra
{
    public class InvocationContext
    {
        public Type ServiceType { get; set; }
        public MethodInfo Method { get; set; }

        public int Timeout { get; set; }
    }
}
