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

        private readonly IContextAccessor _contextAccessor;

        public ServiceActorInterceptor(IServiceProvider provider)
        {
         

            this._contextAccessor = provider.GetService<IContextAccessor>();
           
        }


        public void Intercept(IInvocation invocation)
        {
                     
            if (this._contextAccessor != null)
            {
                if (this._contextAccessor.CallContext == null)
                {
                    this._contextAccessor.CallContext = new CallContext();
                }
                this._contextAccessor.CallContext.AddDef();
            }

            try
            {
                invocation.Proceed();
            }
            catch
            {
                throw;
            }
            finally
            {
                this._contextAccessor?.CallContext.CloseDef();
            }            
        }      
    }
}
