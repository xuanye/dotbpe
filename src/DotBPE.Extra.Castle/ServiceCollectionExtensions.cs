using Microsoft.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using DotBPE.Rpc.Client;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicClientProxy(this IServiceCollection services)
        {
            return services.AddSingleton<ClientInterceptor>()
                .AddSingleton<IProxyGenerator, ProxyGenerator>()
                .AddSingleton<IClientProxy,DynamicClientProxy>();

        }
    }
}
