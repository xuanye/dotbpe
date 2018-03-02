using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public interface IClientProxy
    {
        TClient GetClient<TClient>() where TClient : class, IInvokeClient;       
    }
}
