using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Extra
{
    public class ServiceActorInterceptor:IInterceptor
    {

        private readonly IEnumerable<IRpcServiceInterceptor> Interceptor;
        private readonly IRequestAuditLoggerFactory _requestAuditLoggerFactory;

        public ServiceActorInterceptor(IServiceProvider provider)
        {
            Interceptor = provider.GetServices<IRpcServiceInterceptor>() ?? new IRpcServiceInterceptor[0];

            this._requestAuditLoggerFactory = provider.GetRequiredService<IRequestAuditLoggerFactory>();
        }


        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method.GetCustomAttribute(typeof(RpcMethodAttribute), true);
            if (method == null)
            {
                invocation.Proceed();
                return;
            }

            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            string methodFullName = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            using (var logger = this._requestAuditLoggerFactory.GetLogger(methodFullName))
            {
                logger.SetParameter(invocation.Arguments[0]);

                Process(invocation);

                logger.SetReturnValue(invocation.ReturnValue);
            }
        }

        private void Process(IInvocation invocation)
        {
            if (this.Interceptor.Any())
            {
                this.Interceptor.ForEach(i =>
                {
                    i.Before(invocation);
                });
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (this.Interceptor.Any())
                {
                    this.Interceptor.ForEach(i =>
                    {
                        i.Exception(invocation,ex);
                    });
                }

                throw ex;
            }
            finally
            {
                if (this.Interceptor.Any())
                {
                    this.Interceptor.ForEach(i =>
                    {
                        i.After(invocation);
                    });
                }
            }

        }
    }
}
