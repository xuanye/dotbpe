// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.AuditLog;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DotBPE.BestPractice.AuditLog
{
    public class AuditLogWriter : IAuditLogWriter
    {
        private readonly ILogger _clientLogger;
        private readonly ILogger _serverLogger;
        public AuditLogWriter(ILoggerFactory loggerFactory)
        {
            _clientLogger = loggerFactory.CreateLogger("DotBPE.AuditLog.Client");
            _serverLogger = loggerFactory.CreateLogger("DotBPE.AuditLog.Server");
        }

        public Task WriteAsync(string logTxt, AuditLogType auditLogType)
        {
            var logger = auditLogType == AuditLogType.Client ? _clientLogger : _serverLogger;
            logger.LogInformation(logTxt);

            return Task.CompletedTask;
        }
    }
}
