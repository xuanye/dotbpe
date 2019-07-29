using System.Threading;

namespace Tomato.Rpc.Server
{
    public class DefaultContextAccessor:IContextAccessor
    {
        private static readonly AsyncLocal<ICallContext> _callContextCurrent = new AsyncLocal<ICallContext>();

        public ICallContext CallContext
        {
            get => _callContextCurrent.Value;
            set => _callContextCurrent.Value = value;
        }
    }
}
