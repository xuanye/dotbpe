using Tomato.Rpc.Server;

namespace Tomato.Rpc
{
    public interface IAuditLoggerFormat
    {
        string Format(IRpcContext context,AuditLogType logType,string methodName, object req, RpcResult<object> res, long elapsedMs);
    }
}
