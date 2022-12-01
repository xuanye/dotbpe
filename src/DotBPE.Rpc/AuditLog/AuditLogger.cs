// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Threading.Tasks;

namespace DotBPE.Rpc.AuditLog
{
    public class AuditLogger : IAuditLogger
    {
        private readonly IAuditLogWriter _writer;
        private readonly IAuditLogFormatter _formatter;

        public AuditLogger(AuditLogType auditLogType, IAuditLogWriter writer, IAuditLogFormatter formatter)
        {
            AuditLogType = auditLogType;
            _writer = writer;
            _formatter = formatter;
        }

        public AuditLogType AuditLogType { get; }

        public Task Log(string methodName, object req, object res, int statusCode, long elapsedMS, IRpcContext context)
        {
            try
            {
                var logInfo = new AuditLogInfo()
                {
                    MethodName = methodName,
                    Request = req,
                    Response = res,
                    Context = context,
                    AuditLogType = AuditLogType,
                    StatusCode = statusCode,
                    ElapsedMS = elapsedMS,
                };
                var logText = _formatter.Format(logInfo);

                return _writer.WriteAsync(logText, AuditLogType);
            }
            catch
            {
                //do nothing here
            }
            return Task.CompletedTask;

        }
    }
}
