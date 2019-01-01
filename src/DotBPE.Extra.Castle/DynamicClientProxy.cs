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
            this._generator = provider.GetRequiredService<IProxyGenerator>();
            this._provider = provider;
        }

        protected IInterceptor ClientInterceptor =>
            this._interceptor ??
            (this._interceptor = this._provider.GetRequiredService<ClientInterceptor>());

        public TService Create<TService>() where TService : class
        {
            //TODO:CACHE？
            return this._generator.CreateInterfaceProxyWithoutTarget<TService>(ClientInterceptor);
        }
    }
}
