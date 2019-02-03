using System.Threading;

namespace DotBPE.Rpc.Server.Impl
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
