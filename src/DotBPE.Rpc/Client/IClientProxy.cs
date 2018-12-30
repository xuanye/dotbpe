using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IClientProxy
    {
        TService Create<TService>() where TService : class;
    }
}
