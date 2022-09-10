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
            _contextAccessor = provider.GetService<IContextAccessor>();
        }


        public void Intercept(IInvocation invocation)
        {

            if (_contextAccessor != null)
            {
                if (_contextAccessor.CallContext == null)
                {
                    _contextAccessor.CallContext = new CallContext();
                }
                _contextAccessor.CallContext.AddDef();
            }

            try
            {
                invocation.Proceed();
            }
            finally
            {
               _contextAccessor?.CallContext.CloseDef();
            }
        }
    }
}
