using System;
using Castle.DynamicProxy;
using DotBPE.Rpc.Server;

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
            finally
            {
                this._contextAccessor?.CallContext.CloseDef();
            }            
        }      
    }
}
