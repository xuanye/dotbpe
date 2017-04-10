using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultTransportFactory<TMessage>:ITransportFactory<TMessage> where TMessage:InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultTransportFactory<TMessage>>();
        private readonly IClientBootstrap<TMessage> _bootstrap;

        private readonly ConcurrentDictionary<EndPoint, Lazy<ITransport<TMessage>>> _clients 
            = new ConcurrentDictionary<EndPoint, Lazy<ITransport<TMessage>>>();
        public DefaultTransportFactory(IClientBootstrap<TMessage> bootstrap)
        {
            
            this._bootstrap = bootstrap;
            this._bootstrap.Disconnected += Bootstrap_Disconnected;
        }

        private void Bootstrap_Disconnected(object sender, EndPoint endpoint)
        {
            _clients.TryRemove(endpoint, out var _);
            Logger.Debug("连接已经断开");
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
                            var transportans = new DefaultTransport<TMessage>(context);
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

        public async Task CloseTransport(EndPoint serverAddress)
        {
            try
            {
                if(_clients.ContainsKey(serverAddress))
                {
                    bool success = _clients.TryRemove(serverAddress, out var lazyTransport);
                    if (success && lazyTransport !=null && lazyTransport.IsValueCreated)
                    {
                        await lazyTransport.Value.CloseAsync();
                    }
                }
                    
            }
            catch(Exception ex)
            {
                Logger.Error($"主动关闭连接{serverAddress}时发生异常:" + ex.ToString());
            }
        }
    }
}
