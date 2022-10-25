using DotBPE.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    public class ServiceAuditLogger : AbstractAuditLogger, IRequestAuditLogger
    {
        private readonly ILogger<ServiceAuditLogger> _logger;

        public ServiceAuditLogger(string methodFullName, ILogger<ServiceAuditLogger> logger)
        {
            this.MethodFullName = methodFullName;
            this._logger = logger;
        }



        public override string MethodFullName { get; }

        public void SetContext(IRpcContext context)
        {
            base.Context = context;
        }

        protected override AuditLogType GetAuditLogType()
        {
            return AuditLogType.ServiceReceive;
        }

        protected override ILogger GetTypeLogger()
        {
            return this._logger;
        }
    }
}
