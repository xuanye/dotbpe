using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc {
    public interface IClientProxy {
        TClient GetClient<TClient> () where TClient : class, IInvokeClient;
        TService GetService<TService> ();
    }
}