using DotBPE.Rpc.Server;

namespace DotBPE.Rpc
{
    public interface IAuditLoggerFormat
    {
        string Format(IRpcContext context,AuditLogType logType,string methodName, object req, RpcResult<object> res, long elapsedMs);
    }
}
