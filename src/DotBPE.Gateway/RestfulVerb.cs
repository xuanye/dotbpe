using System;

namespace DotBPE.Gateway
{
    [Flags]
    public enum RestfulVerb
    {
        Get = 1,
        Post =2,
        Put = 4,
        Delete = 8,
        Patch = 16,
        Any = 31
    }
}
