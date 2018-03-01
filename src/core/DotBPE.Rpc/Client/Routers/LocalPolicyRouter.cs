using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Client
{
    public class LocalPolicyRouter<TMessage> : IRouter<TMessage> where TMessage : InvokeMessage
    {
        private static RouterPoint LOCAL_POINT = new RouterPoint() { RoutePointType = RoutePointType.Local };
        public RouterPoint GetRouterPoint(TMessage message)
        {
            return LOCAL_POINT;
        }
    }
}
