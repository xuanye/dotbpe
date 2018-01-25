using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.IntegrationTesting
{
    public class CallContextTestImpl : CallContextTestBase
    {
        readonly ILogger<CallContextTestImpl> Logger;

        private IContextAccessor<AmpMessage> _contextAccessor;
        private readonly string contextKey = "CallContextTestImpl";
        public CallContextTestImpl(IContextAccessor<AmpMessage> contextAccessor,ILogger<CallContextTestImpl> logger)
        {
            _contextAccessor = contextAccessor;
            this.Logger = logger;
        }

        public override async Task<RpcResult<CommonRsp>> TestAsync(VoidReq request)
        {
            var res = new RpcResult<CommonRsp>();

            res.Data = new CommonRsp();

            string randomId = Guid.NewGuid().ToString("D");
            _contextAccessor.CallContext.Set(contextKey, randomId);

            await Task.Factory.StartNew(() => {               
                var objSet = _contextAccessor.CallContext.Get(contextKey);
                if(objSet == null)
                {
                    Logger.LogWarning("get value from context failed");
                    res.Data.Status = -1;
                }
                else
                {
                    string v = objSet.ToString();
                    if(v != randomId)
                    {
                        Logger.LogWarning("get value from context ,but value changed, in new Task");
                    }
                    res.Data.Status = v == randomId ? 0 : -1;
                }
            }
            );
            if(res.Data.Status != 0)
            {
                return res;
            }
            var objSetOut = _contextAccessor.CallContext.Get(contextKey);
            if (objSetOut == null)
            {
                res.Data.Status = -1;
            }
            else
            {
                string v = objSetOut.ToString();
                if (v != randomId)
                {
                    Logger.LogWarning("get value from context ,but value changed");
                }
                res.Data.Status = v == randomId ? 0 : -1;
                Logger.LogDebug("{0}={1}", v, randomId);
            }

            return res;
        }
    }
}
