using DotBPE.Gateway.Internal;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway
{
    internal class HttpApiServiceMethodProvider<TService> : IRpcServiceMethodProvider<TService> where TService : class
    {
        private readonly IClientProxy _clientProxy;
        private readonly IJsonParser _jsonParser;
        private readonly ILoggerFactory _loggerFactory;  

        public HttpApiServiceMethodProvider(
           IClientProxy clientProxy
           ,IJsonParser jsonParser
           , ILoggerFactory loggerFactory        
           )
        {
            _clientProxy = clientProxy;
            _jsonParser = jsonParser;
            _loggerFactory = loggerFactory;          
        }


        public void OnServiceMethodDiscovery(RpcServiceMethodProviderContext<TService> context)
        {
            try
            {
                var binder = new HttpApiProviderServiceBinder<TService>(context, _clientProxy, _jsonParser,_loggerFactory);
                binder.BindAll();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding RPC service To HttpApi '{typeof(TService).Name}'.", ex);
            }
              
        }
    }
}
