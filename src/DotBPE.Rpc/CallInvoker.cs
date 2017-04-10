using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public abstract class CallInvoker<TMessage> where TMessage : InvokeMessage
    {
        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public abstract TMessage BlockingUnaryCall(TMessage request);



        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public abstract Task<TMessage> AsyncUnaryCall(TMessage request);
          
    }
}
