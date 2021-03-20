namespace DotBPE.Rpc.Server
{
    public interface IRequestAuditLoggerFactory
    {

        IRequestAuditLogger GetLogger(string methodFullName);

    }
}
