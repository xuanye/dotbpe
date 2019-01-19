using DotBPE.Protocol.Amp;
using DotBPE.Rpc;

namespace PiggyMetrics.Common
{
    public class DotBPECallContextStorage : Vulcan.DataAccess.IRuntimeContextStorage
    {
        private readonly IContextAccessor<AmpMessage> _contextAccessor;
        public DotBPECallContextStorage(IContextAccessor<AmpMessage> contextAccessor){
            this._contextAccessor = contextAccessor;
        }
        public bool ContainsKey(string key)
        {
           return _contextAccessor.CallContext.ContainsKey(key);
        }

        public object Get(string key)
        {
           return _contextAccessor.CallContext.Get(key);
        }

        public void Remove(string key)
        {
            _contextAccessor.CallContext.Remove(key);
        }

        public void Set(string key, object item)
        {
           _contextAccessor.CallContext.Set(key,item);
        }
    }
}
