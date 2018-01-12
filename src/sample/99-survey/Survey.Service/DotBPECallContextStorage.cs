using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Text;
using Vulcan.DataAccess;

namespace Survey.Service
{
    public class DotBPECallContextStorage<TMessage> : IRuntimeContextStorage where TMessage : InvokeMessage
    {
        private readonly IContextAccessor<TMessage> _contextAccessor;
        public DotBPECallContextStorage(IContextAccessor<TMessage> contextAccessor)
        {
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
            _contextAccessor.CallContext.Set(key, item);
        }
    }
}
