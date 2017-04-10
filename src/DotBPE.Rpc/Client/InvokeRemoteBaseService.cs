using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public abstract class InvokeRemoteBaseService<TMessage> where TMessage:InvokeMessage
    {
        private readonly CallInvoker<TMessage> _callInvoke;
        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="channel">The channel to use for remote call invocation.</param>
        public InvokeRemoteBaseService(IRpcClient<TMessage> client) : this(new DefaultCallInvoker<TMessage>(client))
        {
           
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public InvokeRemoteBaseService(CallInvoker<TMessage> callInvoker) 
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
     
    }
}
