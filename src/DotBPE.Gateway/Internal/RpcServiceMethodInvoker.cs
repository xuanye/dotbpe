using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Internal
{

    /// <summary>
    /// Unary server method invoker.
    /// </summary>
    /// <typeparam name="TService">Service type for this method.</typeparam>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    internal sealed class RpcServiceMethodInvoker<TService, TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where TService : class
    {
        private readonly RpcServiceMethod<TService, TRequest, TResponse> _invoker;
        private readonly RpcServiceMethodWithTimeout<TService, TRequest, TResponse> _invokerWithTimeout;
        private readonly int _timeout;
        private readonly Method<TRequest, TResponse> _method;
        private readonly MethodOptions _options;
        private readonly IClientProxy _clientProxy;

        /// <summary>
        /// Creates a new instance of <see cref="UnaryServerMethodInvoker{TService, TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="invoker">The unary method to invoke.</param>
        /// <param name="method">The description of the RPC method.</param>
        /// <param name="options">The options used to execute the method.</param>
        /// <param name="serviceActivator">The service activator used to create service instances.</param>
        public RpcServiceMethodInvoker(
            RpcServiceMethod<TService, TRequest, TResponse> invoker,
            RpcServiceMethodWithTimeout<TService, TRequest, TResponse> invokerWithTimeout,
            int timeout,
            Method<TRequest, TResponse> method,
            MethodOptions options,
            IClientProxy clientProxy)
           
        {
            _invoker = invoker;
            _invokerWithTimeout = invokerWithTimeout;
            _timeout = timeout;
            _method = method;
            _options = options;
            _clientProxy = clientProxy;
        }

        /// <summary>
        /// Invoke the unary method with the specified <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>      
        /// <param name="request">The <typeparamref name="TRequest"/> message.</param>
        /// <returns>A <see cref="Task{TResponse}"/> that represents the asynchronous method. The <see cref="Task{TResponse}.Result"/>
        /// property returns the <typeparamref name="TResponse"/> message.</returns>
        public async Task<RpcResult<TResponse>> Invoke(HttpContext httpContext,TRequest request)
        {
          
            try
            {
                var instance = _clientProxy.Create<TService>();
                if(_invoker != null)
                {
                    return await _invoker(instance,request);
                }
                else
                {
                    return await _invokerWithTimeout(instance,request,_timeout);
                }
                
            }
            finally
            {
             
            }
        }
    }
}
