using System;
using System.Collections.Concurrent;
using System.Net;
using DotBPE.Rpc.Codes;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultTransportFactory<TMessage>:ITransportFactory<TMessage> where TMessage:InvokeMessage
    {
        private readonly ILogger _logger;
        private readonly IClientBootstrap<TMessage> _bootstrap;

        private readonly ConcurrentDictionary<EndPoint, Lazy<ITransport<TMessage>>> _clients 
            = new ConcurrentDictionary<EndPoint, Lazy<ITransport<TMessage>>>();
        public DefaultTransportFactory(IClientBootstrap<TMessage> bootstrap, ILogger<DefaultTransportFactory<TMessage>> logger)
        {
            this._logger = logger;
            this._bootstrap = bootstrap;
            this._bootstrap.Disconnected += Bootstrap_Disconnected;
        }

        private void Bootstrap_Disconnected(object sender, EndPoint endpoint)
        {
            _clients.TryRemove(endpoint, out var _);
            this._logger.LogDebug("连接已经断开");
        }

        public ITransport<TMessage> CreateTransport(EndPoint endpoint) 
        {
            try
            {
                return _clients.GetOrAdd(endpoint
                    , k => new Lazy<ITransport<TMessage>>(() =>
                        {

                            var bootstrap = _bootstrap;
                            var context = bootstrap.ConnectAsync(k).Result;
                            var transportans = new DefaultTransport<TMessage>(context, _logger);
                            return transportans;
                        }
                    )).Value;
            }
            catch
            {
                _clients.TryRemove(endpoint, out var _);
                throw;
            }
        }

      
    }
}
