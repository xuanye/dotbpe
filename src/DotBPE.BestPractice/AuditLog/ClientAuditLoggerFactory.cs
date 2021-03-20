using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    public class ClientAuditLoggerFactory : IClientAuditLoggerFactory
    {
        private readonly ILogger<ClientAuditLogger> clientLogger;
        //private readonly ILogger<ServiceAuditLogger> serviceLogger;

        public ClientAuditLoggerFactory(ILoggerFactory factory)
        {
            this.clientLogger = factory.CreateLogger<ClientAuditLogger>();
        }
        public IClientAuditLogger GetLogger(string methodFullName)
        {
            return new ClientAuditLogger(methodFullName, this.clientLogger);
        }
    }
}
