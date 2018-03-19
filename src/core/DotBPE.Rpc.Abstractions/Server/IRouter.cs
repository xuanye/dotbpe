using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;

namespace DotBPE.Rpc {
    public interface IRouter<TMessage> where TMessage : InvokeMessage {
        RouterPoint GetRouterPoint (TMessage message);
    }

}