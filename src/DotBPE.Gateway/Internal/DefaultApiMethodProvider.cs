// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway.Internal
{
    internal class DefaultApiMethodProvider<TService> : IApiMethodProvider<TService>
        where TService : class
    {
        private readonly RpcGatewayOption _gatewayOption;
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly ILoggerFactory _loggerFactory;

        public DefaultApiMethodProvider(
           RpcGatewayOption gatewayOption
          , IClientProxy clientProxy
          , IJsonParser jsonParser
          , ILoggerFactory loggerFactory
          )
        {
            _gatewayOption = gatewayOption;
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _loggerFactory = loggerFactory;

        }


        public void OnMethodDiscovery(ApiMethodProviderContext<TService> context)
        {
            try
            {
                var binder = new ApiProviderServiceBinder<TService>(context, _gatewayOption, _clientProxy, _jsonParser, _loggerFactory);
                binder.Bind();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding RPC service To HttpApi '{typeof(TService).Name}'.", ex);
            }
        }
    }
}
