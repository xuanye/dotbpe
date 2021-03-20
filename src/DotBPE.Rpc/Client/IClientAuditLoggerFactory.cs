namespace DotBPE.Rpc.Client
{
    public interface IClientAuditLoggerFactory
    {
        IClientAuditLogger GetLogger(string methodFullName);
    }
}
