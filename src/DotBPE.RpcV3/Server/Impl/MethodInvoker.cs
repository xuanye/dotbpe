// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class MethodInvoker<TService, TRequest, TResponse>
        where TService : class
        where TRequest : class
        where TResponse : class
    {
        private readonly ServiceMethod<TService, TRequest, TResponse>? _invoker;
        private readonly ServiceMethodWithTimeout<TService, TRequest, TResponse>? _invokerWithTimeout;
        private readonly int _timeout;

        public MethodInvoker(
            ServiceMethod<TService, TRequest, TResponse>? invoker,
            ServiceMethodWithTimeout<TService, TRequest, TResponse>? invokerWithTimeout,
            int timeout
            )
        {
            _invoker = invoker;
            _invokerWithTimeout = invokerWithTimeout;
            _timeout = timeout;
        }

        public async Task<RpcResult<TResponse>> InvokeAsync(IServiceActor serviceActor, TRequest? request)
        {
            if (_invoker != null)
                return await _invoker.Invoke(serviceActor, request);
            if (_invokerWithTimeout != null)
                return await _invokerWithTimeout.Invoke(serviceActor, request, _timeout);

            throw new InvalidOperationException("There is no method invoker");
        }
    }
}
