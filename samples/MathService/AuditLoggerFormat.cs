using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathService
{
    public class AuditLoggerFormat : IAuditLoggerFormat
    {        
        public string Format(IRpcContext context,AuditLogType logType, string methodName, object req, RpcResult<object> res, long elapsedMs)
        {
            if(req==null || res == null)
            {
                return string.Format("req or res is null ------------,reqType={0},resType = {1}", req?.GetType().Name, res?.Data?.GetType().Name);
            }
            return string.Format("logType={0},methodName={1}, elapsedMs={2}", logType, methodName, elapsedMs);
        }
    }
}
