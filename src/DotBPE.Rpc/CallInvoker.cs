using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using System;

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public abstract class CallInvoker<TMessage> where TMessage : InvokeMessage
    {
        private readonly IRpcClient<TMessage> _client;

        public CallInvoker(IRpcClient<TMessage> client)
        {
            this._client = client;
            this._client.Recieved += MessageRecieved;
        }

        protected IRpcClient<TMessage> RpcClient
        {
            get
            {
                return this._client;
            }
        }

        protected abstract void MessageRecieved(object sender, MessageRecievedEventArgs<TMessage> e);


        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public abstract TMessage BlockingCall(TMessage request);


        public abstract Task<TMessage> AsyncCall(TMessage request, int timeOut = 3000);
    }
}
