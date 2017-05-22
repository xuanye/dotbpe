using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.IntegrationTesting
{
    public class CallContextTestImpl : CallContextTestBase
    {
        static ILogger Logger = Rpc.Environment.Logger.ForType<CallContextTestImpl>();
        private IContextAccessor<AmpMessage> _contextAccessor;
        private readonly string contextKey = "CallContextTestImpl";
        public CallContextTestImpl(IContextAccessor<AmpMessage> contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public override async Task<CommonRsp> TestAsync(VoidReq request)
        {
            var rsp = new CommonRsp();
            string randomId = Guid.NewGuid().ToString("D");
            _contextAccessor.CallContext.Set(contextKey, randomId);

            await Task.Factory.StartNew(() => {               
                var objSet = _contextAccessor.CallContext.Get(contextKey);
                if(objSet == null)
                {
                    Logger.Warning("get value from context failed");
                    rsp.Status = -1;
                }
                else
                {
                    string v = objSet.ToString();
                    if(v != randomId)
                    {
                        Logger.Warning("get value from context ,but value changed, in new Task");
                    }
                    rsp.Status = v == randomId ? 0 : -1;
                }
            }
            );
            if(rsp.Status !=0)
            {
                return rsp;
            }
            var objSetOut = _contextAccessor.CallContext.Get(contextKey);
            if (objSetOut == null)
            {
                rsp.Status = -1;
            }
            else
            {
                string v = objSetOut.ToString();
                if (v != randomId)
                {
                    Logger.Warning("get value from context ,but value changed");
                }
                rsp.Status = v == randomId ? 0 : -1;
                Logger.Debug("{0}={1}", v, randomId);
            }

            return rsp;
        }
    }
}
