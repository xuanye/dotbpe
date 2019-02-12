using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using Castle.DynamicProxy;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Extra
{
    public class ServiceActorInterceptor:IInterceptor
    {

        private readonly IEnumerable<IRpcServiceInterceptor> _interceptors;
        private readonly IRequestAuditLoggerFactory _requestAuditLoggerFactory;

        private readonly IContextAccessor _contextAccessor;

        public ServiceActorInterceptor(IServiceProvider provider)
        {
            this._interceptors = provider.GetServices<IRpcServiceInterceptor>() ?? new IRpcServiceInterceptor[0];

            this._contextAccessor = provider.GetService<IContextAccessor>();

            this._requestAuditLoggerFactory = provider.GetRequiredService<IRequestAuditLoggerFactory>();
        }


        public void Intercept(IInvocation invocation)
        {
            /*
             * commet by xuanye
            var method = invocation.Method.GetCustomAttribute(typeof(RpcMethodAttribute), true);
            if (method == null)
            {
                invocation.Proceed();
                return;
            }
            */
            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            var methodFullName = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            if (this._contextAccessor != null)
            {
                if (this._contextAccessor.CallContext == null)
                {
                    this._contextAccessor.CallContext = new CallContext();
                }
                this._contextAccessor.CallContext.AddDef();
            }
            using (var logger = this._requestAuditLoggerFactory.GetLogger(methodFullName))
            {
                logger.SetParameter(invocation.Arguments[0]);

                Process(invocation);

                logger.SetReturnValue(invocation.ReturnValue);
            }

            this._contextAccessor?.CallContext.CloseDef();
        }

        private void Process(IInvocation invocation)
        {
            if (this._interceptors.Any())
            {
                this._interceptors.ForEach(i =>
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
                if (this._interceptors.Any())
                {
                    this._interceptors.ForEach(i =>
                    {
                        i.Exception(invocation,ex);
                    });
                }

                throw ex;
            }
            finally
            {
                if (this._interceptors.Any())
                {
                    this._interceptors.ForEach(i =>
                    {
                        i.After(invocation);
                    });
                }
            }

        }
    }
}
