using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public abstract class InvokeClientBase<TMessage>:IDisposable where TMessage:InvokeMessage
    {
        private readonly CallInvoker<TMessage> _callInvoke;
      
        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public InvokeClientBase(CallInvoker<TMessage> callInvoker) 
        {

            this._callInvoke = callInvoker;
        }

        protected CallInvoker<TMessage> CallInvoker
        {
            get
            {
                return _callInvoke;
            }
        }

        public void Dispose()
        {
            this._callInvoke?.Dispose();
        }
    }
}
