using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IRpcService
    {
        object Invoke(ushort messageId,params object[] args);
    }
}
