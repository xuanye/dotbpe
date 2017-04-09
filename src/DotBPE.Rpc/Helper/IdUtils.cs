using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Helper
{
    public static class IdUtils
    {
        public static string NewId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
