using Castle.DynamicProxy;
using DotBPE.Rpc.Client;

namespace DotBPE.Extra
{
    public class LocalInvokeInterceptor:IInterceptor
    {
        private readonly IClientAuditLoggerFactory _auditLoggerFactory;

        public LocalInvokeInterceptor(IClientAuditLoggerFactory auditLogFactory)
        {
            this._auditLoggerFactory = auditLogFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            string methodFullName = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            using (var logger = this._auditLoggerFactory.GetLogger(methodFullName))
            {
                logger.SetParameter(invocation.Arguments[0]);
                invocation.Proceed();
                logger.SetReturnValue(invocation.ReturnValue);
            }

        }
    }
}
