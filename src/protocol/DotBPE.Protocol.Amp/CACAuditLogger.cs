using DotBPE.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DotBPE.Protocol.Amp
{
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
