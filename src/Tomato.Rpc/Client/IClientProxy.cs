using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc.Client
{
    public interface IClientProxy
    {
        TService Create<TService>(ushort specialMessageId=0) where TService : class;
    }
}
