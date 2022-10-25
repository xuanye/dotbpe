// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class ServiceAuditLogInterceptor : Interceptor
    {
        private readonly IAuditLoggerFactory _auditLoggerFactory;

        public ServiceAuditLogInterceptor(IAuditLoggerFactory auditLoggerFactory = null)
        {
            _auditLoggerFactory = auditLoggerFactory;
        }

        protected override async Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, InvocationContext context, ServiceMethod<TRequest, TResponse> continuation)
        {
            RpcResult<TResponse> result = null;
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                result = await base.ServiceHandle(req, context, continuation);
            }
            finally
            {
                if (result == null)
                {
                    result = new RpcResult<TResponse>() { Code = RpcStatusCodes.CODE_INTERNAL_ERROR };
                }
            }
            sw.Stop();
            if (_auditLoggerFactory == null)
            {
                var methodName = $"{context.Method.DeclaringType.Name}.{context.Method.Name}";
                var logger = _auditLoggerFactory.GetLogger(AuditLogType.InProc);
                if (logger != null)
                {
                    await logger.Log(methodName, req, result?.Data, result.Code, sw.ElapsedMilliseconds, LocalRpcContext.Instance);
                }
            }
            return result;
        }

    }
}
