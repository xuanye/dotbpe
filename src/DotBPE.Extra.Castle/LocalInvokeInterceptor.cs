using Castle.DynamicProxy;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;

namespace DotBPE.Extra
{
    public class LocalInvokeInterceptor:IInterceptor
    {
        private readonly IClientAuditLoggerFactory _clientAuditLogger;
        private readonly IRequestAuditLoggerFactory _requestAuditLogger;
      
        public LocalInvokeInterceptor(IClientAuditLoggerFactory clientAuditLogger, IRequestAuditLoggerFactory requestAuditLogger)
        {
            this._clientAuditLogger = clientAuditLogger;
            this._requestAuditLogger = requestAuditLogger;
        }

        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.Name.Split('`');
            var methodFullName = $"{serviceNameArr[0]}.{invocation.Method.Name}";


            using (var loggerClient = this._clientAuditLogger.GetLogger(methodFullName))
            using (var loggerRequest = this._requestAuditLogger.GetLogger(methodFullName))
            {
                loggerClient.SetParameter(invocation.Arguments[0]);
                loggerRequest.SetParameter(invocation.Arguments[0]);
                loggerRequest.SetContext(LocalRpcContext.Local);
                invocation.Proceed();
                loggerClient.SetReturnValue(invocation.ReturnValue);
                loggerRequest.SetReturnValue(invocation.ReturnValue);
            }

        }
    }
}
