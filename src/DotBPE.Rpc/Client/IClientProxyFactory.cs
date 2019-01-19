using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Client
{
    public interface IClientProxyFactory
    {
        IClientProxy GetProxyInstance();

        IClientProxyFactory AddDependencyServices(Action<IServiceCollection> configServicesDelegate);


        IClientProxyFactory Configure<TOption>(Action<TOption> configureOptions) where TOption : class;

    }
}
