using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace DotBPE.Extra
{
    public class DynamicClientProxy : IClientProxy
    {
        private readonly IProxyGenerator _generator;
        private readonly ClientInterceptor _interceptor;

     

        public DynamicClientProxy(IServiceProvider provider)
        {
            _generator = provider.GetRequiredService<IProxyGenerator>();
            _interceptor = provider.GetRequiredService<ClientInterceptor>();
        }

        public TService Create<TService>() where TService : class,IRpcService
        {
           return _generator.CreateInterfaceProxyWithoutTarget<TService>(_interceptor);
        }
    }

  
}
