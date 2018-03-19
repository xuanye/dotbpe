using DotBPE.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protocol.Amp
{
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
