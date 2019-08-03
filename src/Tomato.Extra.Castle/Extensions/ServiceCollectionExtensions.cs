using Microsoft.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using Tomato.Rpc.Client;
using Tomato.Rpc.Protocol;
using Tomato.Rpc.Server;

namespace Tomato.Extra
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicClientProxy(this IServiceCollection services)
        {
            return services.AddSingleton<RemoteInvokeInterceptor>()
                .AddSingleton<LocalInvokeInterceptor>() //客户端拦截器，处理调用本地的请求
                .AddSingleton<RemoteInvokeInterceptor>() //客户端拦截器，处理调用远端的请求
                .AddSingleton<IProxyGenerator, ProxyGenerator>()
                .AddSingleton<IClientProxy, DynamicClientProxy>()
                .AddSingleton<ServiceActorInterceptor>(); //ServiceActor拦截器

        }

        public static IServiceCollection AddDynamicServiceProxy(this IServiceCollection services)
        {
            return services.AddSingleton<IServiceActorLocator<AmpMessage>,DynamicServiceActorLocator>();
        }
    }
}
