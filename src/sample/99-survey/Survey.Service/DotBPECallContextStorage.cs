using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Vulcan.DataAccess;

namespace Survey.Service
{
    public class DotBPECallContextStorage<TMessage> : IRuntimeContextStorage where TMessage : InvokeMessage
    {
        private readonly IContextAccessor<TMessage> _contextAccessor;
        private ILogger<DotBPECallContextStorage<TMessage>> _logger;
        public DotBPECallContextStorage(IContextAccessor<TMessage> contextAccessor,ILogger<DotBPECallContextStorage<TMessage>> logger)
        {
            this._contextAccessor = contextAccessor;
            this._logger = logger;
        }
        public bool ContainsKey(string key)
        {
           
            var has = _contextAccessor.CallContext.ContainsKey(key);
            //this._logger.LogDebug("CallContext.ContainsKey key:{0},res ={1}", key,has);
            return has;

        }

        public object Get(string key)
        {
            //this._logger.LogDebug("CallContext.Get key:{0}", key);
            return _contextAccessor.CallContext.Get(key);
        }

        public void Remove(string key)
        {
            //this._logger.LogDebug("CallContext.Remove key:{0}", key);
            _contextAccessor.CallContext.Remove(key);
        }

        public void Set(string key, object item)
        {
            //this._logger.LogDebug("CallContext.Set key:{0}", key);
            _contextAccessor.CallContext.Set(key, item);
        }
    }
}
