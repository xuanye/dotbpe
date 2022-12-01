// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Reflection;

namespace DotBPE.Extra
{
    public class InvokeMeta
    {
        public int ServiceId { get; set; }
        public ushort MessageId { get; set; }

        public string ServiceGroupName { get; set; }

        public MethodInfo InvokeMethod { get; set; }

    }
}
