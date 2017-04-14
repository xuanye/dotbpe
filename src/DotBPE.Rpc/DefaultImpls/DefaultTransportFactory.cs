using System;
using System.Collections.Generic;
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

        private readonly Dictionary<EndPoint, Lazy<ITransport<TMessage>>> _clients
            = new Dictionary<EndPoint, Lazy<ITransport<TMessage>>>();

        private static readonly object lockObj = new object();
        public DefaultTransportFactory(IClientBootstrap<TMessage> bootstrap)
        {

            this._bootstrap = bootstrap;
            this._bootstrap.Disconnected += Bootstrap_Disconnected;
        }

        private bool RemoveTransport(EndPoint key,out Lazy<ITransport<TMessage>> value){
            bool s = false;
            value = null;
            lock(lockObj){
                if(this._clients.ContainsKey(key)){
                    value = this._clients[key];
                    this._clients.Remove(key);
                    s = true;
                }
                else{
                    Logger.Debug("什么没有这个地址{0}",key);
                }
            }
            return s;
        }
        private Lazy<ITransport<TMessage>> GetOrAdd(EndPoint key,Func<EndPoint,Lazy<ITransport<TMessage>>> createAction){
            Lazy<ITransport<TMessage>> value = null;
            lock(lockObj){
                if(this._clients.ContainsKey(key)){
                    value = this._clients[key];
                }
            }
            if(value!=null){
                return value;
            }
            lock(lockObj){
                if(!this._clients.ContainsKey(key)){
                    value = createAction.Invoke(key);
                    this._clients.Add(key,value);
                }
            }
            if(value!=null){
                return value;
            }
            return GetOrAdd(key,createAction);
        }
        private void Bootstrap_Disconnected(object sender, EndPoint endpoint)
        {
            var removed = RemoveTransport(endpoint, out var _);

            Logger.Debug("连接{0}已经断开,移除ITransport{1},当前连接数量{2}",endpoint,removed?"成功":"失败",_clients.Keys.Count);
        }

        public ITransport<TMessage> CreateTransport(EndPoint endpoint)
        {
            try
            {
                return GetOrAdd(endpoint
                    , k => new Lazy<ITransport<TMessage>>(() =>
                        {
                            var context = _bootstrap.ConnectAsync(k).Result;
                            var transportans = new DefaultTransport<TMessage>(context);
                              Logger.Debug("连接{0},创建ITransport成功",endpoint);
                            return transportans;
                        }
                    )).Value;
            }
            catch
            {
                RemoveTransport(endpoint, out var _);
                throw;
            }
        }

        public async Task CloseTransportAsync(EndPoint serverAddress)
        {
            try
            {
                if(_clients.ContainsKey(serverAddress))
                {
                    bool success = RemoveTransport(serverAddress, out var lazyTransport);
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
