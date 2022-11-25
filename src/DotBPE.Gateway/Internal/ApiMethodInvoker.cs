// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Internal
{
    internal class ApiMethodInvoker<TService, TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where TService : class
    {
        private readonly ServiceMethod<TService, TRequest, TResponse> _invoker;
        private readonly ServiceMethodWithTimeout<TService, TRequest, TResponse> _invokerWithTimeout;
        private readonly int _timeout;
        private readonly IClientProxy _clientProxy;

        public ApiMethodInvoker(
            ServiceMethod<TService, TRequest, TResponse> invoker,
            ServiceMethodWithTimeout<TService, TRequest, TResponse> invokerWithTimeout,
            int timeout,
            IClientProxy clientProxy
            )
        {
            _invoker = invoker;
            _invokerWithTimeout = invokerWithTimeout;
            _timeout = timeout;
            _clientProxy = clientProxy;
        }


        public async Task<RpcResult<TResponse>> Invoke(TRequest request)
        {
            var instance = _clientProxy.Create<TService>();
            if (_invoker != null)
            {
                return await _invoker(instance, request);
            }
            else
            {
                return await _invokerWithTimeout(instance, request, _timeout);
            }
        }
    }
}
