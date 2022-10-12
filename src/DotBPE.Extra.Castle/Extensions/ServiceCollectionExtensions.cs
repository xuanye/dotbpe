// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotBPE.Extra
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDynamicClientProxy(this IServiceCollection services)
        {
            return services.AddSingleton<RemoteInvokeInterceptor>()
                .AddSingleton<IProxyGenerator, ProxyGenerator>()
                .AddSingleton<IClientProxy, DynamicClientProxy>();

        }

        internal static IServiceCollection AddDynamicServiceProxy(this IServiceCollection services)
        {
            services.TryAddSingleton<IContextAccessor, DefaultContextAccessor>();
            return services.AddSingleton<IServiceActorLocator, DynamicServiceActorLocator>()
                .AddServiceInterceptor<CallContextServiceActorInterceptor>();
        }

        public static IServiceCollection AddDynamicProxy(this IServiceCollection services)
        {
            return services.AddDynamicClientProxy().AddDynamicServiceProxy();
        }

        public static IServiceCollection AddServiceInterceptor(this IServiceCollection services, Interceptor interceptor)
        {
            return services.AddSingleton(interceptor);
        }
        public static IServiceCollection AddServiceInterceptor<TInterceptor>(this IServiceCollection services)
            where TInterceptor : Interceptor
        {
            return services.AddSingleton<Interceptor, TInterceptor>();
        }
        public static IServiceCollection AddClientInterceptor(this IServiceCollection services, ClientInterceptor interceptor)
        {
            return services.AddSingleton(interceptor);
        }

        public static IServiceCollection AddClientInterceptor<TInterceptor>(this IServiceCollection services)
           where TInterceptor : ClientInterceptor
        {
            return services.AddSingleton<ClientInterceptor, TInterceptor>();
        }
    }
}
