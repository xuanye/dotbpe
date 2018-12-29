using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public class DefaultClientProxy : IClientProxy
    {
        public DefaultClientProxy()
        {

        }

        public TService Create<TService>() where TService : class,IRpcService
        {
            throw new NotImplementedException();
        }
    }
}
