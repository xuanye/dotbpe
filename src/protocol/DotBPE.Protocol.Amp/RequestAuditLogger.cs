using DotBPE.Rpc;
using Microsoft.Extensions.Logging;

namespace DotBPE.Protocol.Amp
{
    /// <summary>
    /// 请求日志记录类，该类会尝试记录所有接收到的请求日志
    /// </summary>
    /// <seealso cref="DotBPE.Protocol.Amp.AbstractAuditLogger" />
    public class RequestAuditLogger : AbstractAuditLogger
    {
        private static ILogger _Logger;

        protected ILogger Logger
        {
            get
            {
                if (_Logger == null)
                {
                    if (Rpc.Environment.LoggerFactory != null)
                    {
                        _Logger = Rpc.Environment.LoggerFactory.CreateLogger<RequestAuditLogger>();
                    }
                }
                return _Logger;
            }
        }

        protected override AuditLogType GetAuditLogType()
        {
            return AuditLogType.RequestAudit;
        }

        protected override ILogger GetTypeLogger()
        {
            return Logger;
        }
    }
}
