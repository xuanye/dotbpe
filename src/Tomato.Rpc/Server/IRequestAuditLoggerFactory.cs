namespace Tomato.Rpc.Server
{
    public interface IRequestAuditLoggerFactory
    {

        IRequestAuditLogger GetLogger(string methodFullName);

    }
}
