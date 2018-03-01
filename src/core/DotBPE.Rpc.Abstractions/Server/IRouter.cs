using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public interface IRouter<TMessage> where TMessage : InvokeMessage
    {
        RouterPoint GetRouterPoint(TMessage message);
    }
   
}
