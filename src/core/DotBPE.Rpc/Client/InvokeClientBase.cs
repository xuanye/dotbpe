namespace DotBPE.Rpc.Client
{
    public class InvokeClientBase<TMessage> : IInvokeClient where TMessage : InvokeMessage
    {
        private readonly ICallInvoker<TMessage> _callInvoke;

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public InvokeClientBase(ICallInvoker<TMessage> callInvoker)
        {
            this._callInvoke = callInvoker;
        }

        protected ICallInvoker<TMessage> CallInvoker
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
