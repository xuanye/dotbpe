using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    public class ServiceAuditLoggerFactory : IRequestAuditLoggerFactory
    {

        private readonly ILogger<ServiceAuditLogger> serviceLogger;

        public ServiceAuditLoggerFactory(ILoggerFactory factory)
        {
            this.serviceLogger = factory.CreateLogger<ServiceAuditLogger>();
        }

        public IRequestAuditLogger GetLogger(string methodFullName)
        {
            return new ServiceAuditLogger(methodFullName, this.serviceLogger);
        }
    }
}
