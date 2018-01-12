using DotBPE.Rpc.Codes;
using System.Threading;

namespace DotBPE.Rpc
{
    public interface IContextAccessor<TMessage> where TMessage : InvokeMessage
    {
        CallContext<TMessage> CallContext { get; set; }
    }

    public class DefaultContextAccessor<TMessage> : IContextAccessor<TMessage> where TMessage : InvokeMessage
    {
        private static AsyncLocal<CallContext<TMessage>> _callContextCurrent = new AsyncLocal<CallContext<TMessage>>();

        public CallContext<TMessage> CallContext
        {
            get
            {
                return _callContextCurrent.Value;
            }
            set
            {
                _callContextCurrent.Value = value;
            }
        }
    }
}
