using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public interface IRpcInterceptorActivator
    {
        /// <summary>
        /// Creates an interceptor.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="interceptorRegistration">The arguments to pass to the interceptor type instance's constructor.</param>
        /// <returns>The created interceptor.</returns>
        RpcActivatorHandle<Interceptor> Create (IServiceProvider serviceProvider, InterceptorRegistration interceptorRegistration);

        /// <summary>
        /// Releases the specified interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor to release.</param>
        ValueTask ReleaseAsync (RpcActivatorHandle<Interceptor> interceptor);
    }
    /// <summary>
    /// A <typeparamref name="TInterceptor" /> activator abstraction.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
    public interface IRpcInterceptorActivator<TInterceptor> : IRpcInterceptorActivator where TInterceptor : Interceptor
    {
    }

}
