using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public abstract class AbstractCallInvoker<TMessage> : ICallInvoker<TMessage> where TMessage : InvokeMessage
    {
        public AbstractCallInvoker(IRpcClient<TMessage> client)
        {
            this.RpcClient = client;
            this.RpcClient.Recieved -= MessageRecieved;
            this.RpcClient.Recieved += MessageRecieved;
        }

        public IRpcClient<TMessage> RpcClient { get; }

        protected abstract void MessageRecieved(object sender, MessageRecievedEventArgs<TMessage> e);

        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public abstract TMessage BlockingCall(TMessage request, int timeOut = 3000);

        /// <summary>
        /// Asynchronouses the call.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="timeOut">The time out.</param>
        /// <returns></returns>
        public abstract Task<TMessage> AsyncCall(TMessage request, int timeOut = 3000);

        public void Dispose()
        {
            this.RpcClient.Dispose();
        }
    }
}
