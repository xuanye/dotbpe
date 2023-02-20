// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server.Impl
{
    public class DefaultDynamicInvoker<TService, TRequest, TResponse> : IDynamicInvoker
        where TService : class
        where TRequest : class
        where TResponse : class
    {
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly MethodInvoker<TService, TRequest, TResponse> _invokerCore;

        public DefaultDynamicInvoker(IClientProxy clientProxy, IJsonParser jsonParser, MethodInvoker<TService, TRequest, TResponse> invokerCore)
        {
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _invokerCore = invokerCore;
        }
        public int ServiceId { get; }

        public ushort MessageId { get; }

        public Type ServiceType { get; }

        public MethodInfo MethodHandler { get; }

        public async Task<RpcResult<string>> InvokeAsync(string json)
        {
            var serviceInstance = _clientProxy.Create<TService>();
            var request = _jsonParser.FromJson<TRequest>(json);
            var invokerResult = await _invokerCore.InvokeAsync(serviceInstance, request);

            if (invokerResult != null)
            {
                var result = new RpcResult<string>
                {
                    Code = invokerResult.Code
                };
                if (invokerResult.Data != null)
                {
                    result.Data = _jsonParser.ToJson<TResponse>(invokerResult.Data);
                }
                return result;
            }

            throw new Exceptions.RpcException("Internal error, call result is null.");
        }
    }
}
