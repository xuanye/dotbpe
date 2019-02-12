namespace DotBPE.Rpc
{
    public interface IAuditLoggerFormat
    {
        string Format(AuditLogType logLogType,string methodName, object req, RpcResult<object> res, long elapsedMs);
    }
}
