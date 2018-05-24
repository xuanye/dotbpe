using DotBPE.Rpc;
using Microsoft.Extensions.Logging;

namespace DotBPE.Protocol.Amp
{
    /// <summary>
    /// 调用服务日志记录类，该类会尝试记录所有对外发起的请求日志
    /// </summary>
    /// <seealso cref="DotBPE.Protocol.Amp.AbstractAuditLogger" />
    public class CACAuditLogger : AbstractAuditLogger
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
                        _Logger = Rpc.Environment.LoggerFactory.CreateLogger<CACAuditLogger>();
                    }
                }
                return _Logger;
            }
        }

        protected override AuditLogType GetAuditLogType()
        {
            return AuditLogType.CACAduit;
        }

        protected override ILogger GetTypeLogger()
        {
            return Logger;
        }
    }
}
