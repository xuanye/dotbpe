using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    public class ClientAuditLogger : AbstractAuditLogger, IClientAuditLogger
    {
        private readonly ILogger<ClientAuditLogger> _logger;

        public ClientAuditLogger(string methodFullName, ILogger<ClientAuditLogger> logger)
        {
            this._logger = logger;
            this.MethodFullName = methodFullName;
        }



        public override string MethodFullName { get; }

        protected override AuditLogType GetAuditLogType()
        {
            return AuditLogType.ClientCall;
        }

        protected override ILogger GetTypeLogger()
        {
            return this._logger;
        }
    }
}
