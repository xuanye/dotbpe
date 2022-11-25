// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Gateway
{
    [Flags]
    public enum HttpVerb
    {
        UnKnown = 0,
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8,
        Patch = 16,
        Any = 31
    }
}
