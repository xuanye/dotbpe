// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.AuditLog
{
    public interface IAuditLoggerFactory
    {
        IAuditLogger GetLogger(AuditLogType auditLogType);
    }

    public class AuditLoggerFactory : IAuditLoggerFactory
    {
        private readonly IAuditLogger _clientLogger;
        private readonly IAuditLogger _serviceLogger;
        public AuditLoggerFactory(IAuditLogWriter writer, IAuditLogFormatter formatter)
        {
            _clientLogger = new AuditLogger(AuditLogType.Client, writer, formatter);
            _serviceLogger = new AuditLogger(AuditLogType.Service, writer, formatter);
        }
        public IAuditLogger GetLogger(AuditLogType auditLogType)
        {
            return auditLogType == AuditLogType.Client ? _clientLogger : _serviceLogger;
        }
    }
}
