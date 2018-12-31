using Castle.DynamicProxy;
using DotBPE.Rpc.Client;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Extra
{
    public class DynamicClientProxy : IClientProxy
    {
        private readonly IProxyGenerator _generator;
        private ClientInterceptor _interceptor;
        private readonly IServiceProvider _provider;


        public DynamicClientProxy(IServiceProvider provider)
        {
            _generator = provider.GetRequiredService<IProxyGenerator>();
            _provider = provider;
        }

        protected IInterceptor ClientInterceptor
        {
            get
            {
                if(_interceptor == null)
                {
                    _interceptor = _provider.GetRequiredService<ClientInterceptor>();
                }
                return _interceptor;
            }
        }

        public TService Create<TService>() where TService : class
        {
           return _generator.CreateInterfaceProxyWithoutTarget<TService>(ClientInterceptor);
        }

       
    }

  
}
