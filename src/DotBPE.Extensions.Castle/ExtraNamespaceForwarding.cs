// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Extra
{
    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.InvocationContext instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.InvocationContext instead")]
    public class InvocationContext : DotBPE.Extensions.InvocationContext
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.InvokeMeta instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.InvokeMeta instead")]
    public class InvokeMeta : DotBPE.Extensions.InvokeMeta
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ServiceMethod&lt;TRequest, TResponse&gt; instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ServiceMethod<TRequest, TResponse> instead")]
    public delegate Task<DotBPE.Rpc.RpcResult<TResponse>> ServiceMethod<TRequest, TResponse>(TRequest request, DotBPE.Extensions.InvocationContext callContext)
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.Interceptor instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.Interceptor instead")]
    public abstract class Interceptor : DotBPE.Extensions.Interceptor
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ClientInterceptor instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ClientInterceptor instead")]
    public abstract class ClientInterceptor : DotBPE.Extensions.ClientInterceptor
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ServiceAuditLogInterceptor instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ServiceAuditLogInterceptor instead")]
    public class ServiceAuditLogInterceptor : DotBPE.Extensions.ServiceAuditLogInterceptor
    {
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.RemoteInvokeInterceptor instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.RemoteInvokeInterceptor instead")]
    public class RemoteInvokeInterceptor : DotBPE.Extensions.RemoteInvokeInterceptor
    {
        public RemoteInvokeInterceptor(ICallInvoker callInvoker) : base(callInvoker)
        {
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.DynamicServiceActorLocator instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.DynamicServiceActorLocator instead")]
    public class DynamicServiceActorLocator : DotBPE.Extensions.DynamicServiceActorLocator
    {
        public DynamicServiceActorLocator(
            Castle.DynamicProxy.IProxyGenerator generator,
            IEnumerable<DotBPE.Extensions.Interceptor> interceptors,
            IEnumerable<IServiceActor> actors,
            ILoggerFactory loggerFactory) : base(generator, interceptors, actors, loggerFactory)
        {
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.DynamicClientProxy instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.DynamicClientProxy instead")]
    public class DynamicClientProxy : DotBPE.Extensions.DynamicClientProxy
    {
        public DynamicClientProxy(
            Castle.DynamicProxy.IProxyGenerator generator,
            IServiceRouter router,
            DotBPE.Extensions.RemoteInvokeInterceptor interceptor,
            IServiceProvider serviceProvider) : base(generator, router, interceptor, serviceProvider)
        {
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.CallContextServiceActorInterceptor instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.CallContextServiceActorInterceptor instead")]
    public class CallContextServiceActorInterceptor : DotBPE.Extensions.CallContextServiceActorInterceptor
    {
        public CallContextServiceActorInterceptor(IContextAccessor contextAccessor) : base(contextAccessor)
        {
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions instead")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddDynamicProxy instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddDynamicProxy instead")]
        public static IServiceCollection AddDynamicProxy(this IServiceCollection services)
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddDynamicProxy(services);
        }

        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor instead")]
        public static IServiceCollection AddServiceInterceptor(this IServiceCollection services, Interceptor interceptor)
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor(services, interceptor);
        }

        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor instead")]
        public static IServiceCollection AddServiceInterceptor<TInterceptor>(this IServiceCollection services)
            where TInterceptor : Interceptor
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddServiceInterceptor<TInterceptor>(services);
        }

        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor instead")]
        public static IServiceCollection AddClientInterceptor(this IServiceCollection services, ClientInterceptor interceptor)
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor(services, interceptor);
        }

        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor instead")]
        public static IServiceCollection AddClientInterceptor<TInterceptor>(this IServiceCollection services)
           where TInterceptor : ClientInterceptor
        {
            return DotBPE.Extensions.ServiceCollectionExtensions.AddClientInterceptor<TInterceptor>(services);
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.HostBuilderExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions instead")]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.HostBuilderExtensions.UseCastleDynamicProxy instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions.UseCastleDynamicProxy instead")]
        public static Microsoft.Extensions.Hosting.IHostBuilder UseCastleDynamicProxy(this Microsoft.Extensions.Hosting.IHostBuilder @this)
        {
            return DotBPE.Extensions.HostBuilderExtensions.UseCastleDynamicProxy(@this);
        }
    }

    /// <summary>
    /// This type is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions instead")]
    public static class ClientProxyFactoryExtensions
    {
        /// <summary>
        /// This method is obsolete. Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseCastleDynamicClientProxy instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.ClientProxyFactoryExtensions.UseCastleDynamicClientProxy instead")]
        public static IClientProxyFactory UseCastleDynamicClientProxy(this IClientProxyFactory @this)
        {
            return DotBPE.Extensions.ClientProxyFactoryExtensions.UseCastleDynamicClientProxy(@this);
        }
    }
}
