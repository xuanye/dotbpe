using System;
using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Client
{
    public interface IClientProxyFactory
    {
        IClientProxy GetClientProxy();

        IClientProxyFactory AddDependencyServices(Action<IServiceCollection> configServicesDelegate);


        IClientProxyFactory Configure<TOption>(Action<TOption> configureOptions) where TOption : class;

        TService GetService<TService>() where TService : class;

    }
}
