using Tomato.Rpc;
using Tomato.Rpc.Client;
using Tomato.Rpc.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathService
{
    public class RequestAuditLoggerFactory : IRequestAuditLoggerFactory
    {       
        private readonly ILogger<RequestAuditLogger> serviceLogger;

        public RequestAuditLoggerFactory(ILoggerFactory factory)
        {
            this.serviceLogger = factory.CreateLogger<RequestAuditLogger>();
        }

        public IRequestAuditLogger GetLogger(string methodFullName)
        {
            return new RequestAuditLogger(methodFullName, this.serviceLogger);
        }      
    }

    public class ClientAuditLoggerFactory :  IClientAuditLoggerFactory
    {

        private readonly ILogger<ClientAuditLogger> clientLogger;

        public ClientAuditLoggerFactory(ILoggerFactory factory)
        {
            this.clientLogger = factory.CreateLogger<ClientAuditLogger>();
        }

        public IClientAuditLogger GetLogger(string methodFullName)
        {
            return new ClientAuditLogger(methodFullName, this.clientLogger);
        }
    }

    public class RequestAuditLogger : AbstractAuditLogger, IRequestAuditLogger
    {
        private readonly ILogger _logger;

        public RequestAuditLogger(string methodName,ILogger logger)
        {
            this.MethodFullName = methodName;
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

    public class ClientAuditLogger : AbstractAuditLogger, IClientAuditLogger
    {
        private readonly ILogger _logger;
        public ClientAuditLogger(string methodName, ILogger logger)
        {
            this.MethodFullName = methodName;
            this._logger = logger;
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
