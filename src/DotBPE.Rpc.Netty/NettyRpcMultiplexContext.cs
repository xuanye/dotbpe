using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;

namespace DotBPE.Rpc.Netty
{
    public class NettyRpcMultiplexContext<TMessage> : IRpcContext<TMessage> where TMessage : IMessage
    {
        ILogger Logger  = Environment.Logger.ForType< NettyRpcMultiplexContext<TMessage>>();
        private readonly IMessageCodecs<TMessage> _codecs;
        private  readonly Bootstrap _bootstrap ;
        private Endpoint _remoteAddress;
        private List<IChannel> _channels = new List<IChannel>();

        private readonly bool _autoReConnect = true;

        private static object  _lockObj = new object();
        public NettyRpcMultiplexContext(Bootstrap bootstrap, IMessageCodecs<TMessage> codecs)
        {
            this._bootstrap = bootstrap;
            this._codecs = codecs;
        }
        public Task CloseAsync()
        {
            _channels.ForEach( async (channel)=>{
                if(channel.Open && channel.Active)
                {
                    await channel.CloseAsync();
                }
            });
            _channels.Clear();
            return Task.CompletedTask;
        }

        public Task SendAsync(TMessage data)
        {
            var channel = TryGetOneRandom();
            if(channel == null){
                throw new Exceptions.RpcException("获取channel失败");
            }
            if( channel.Open ){
                return channel.WriteAndFlushAsync(data);
            }
            else{
                TryRemove(channel); // 移除无用的Channel
                StartConnect(channel.RemoteAddress); //启动自动重连
                return SendAsync(data); // 重新调用一次 ，直到链接被移除完
            }
        }
        private IByteBuffer GetBuffer(IChannel channel, TMessage message)
        {
            var buff = channel.Allocator.Buffer(message.Length);
            IBufferWriter writer = NettyBufferManager.CreateBufferWriter(buff);
            this._codecs.Encode(message, writer);
            return buff;
        }

        public  Task InitAsync(EndPoint endpoint,RpcClientOption clientOption)
        {
            _remoteAddress  = endpoint;
            return CreateConnection(endpoint,clientOption.MultiplexCount);
        }
        private async Task CreateConnection(EndPoint endpoint,int count)
        {
            if(count >1){
                Task[] tasks= new Task[count];
                for(var i =0;i<count ;i++)
                {
                   var channel =  await this._bootstrap.ConnectAsync(endpoint);
                   _channels.Add(channel);
                }
            }
            else{
               IChannel channel = await this._bootstrap.ConnectAsync(endpoint);
               _channels.Add(channel);
            }
        }

        internal void BindDisconnect(EventHandler<DisConnectedArgs> disConnected)
        {
            disConnected += Channel_DisConnected ;
        }

        private void Channel_DisConnected(object sender ,DisConnectedArgs args){
            TryRemoveById(args.ContextId);
            StartConnect(args.EndPoint);
        }

        private void TryRemoveById(string id){
            lock(_lockObj){
                var channel = _channels.Find( x => x.Id.AsLongText() == id) ;
                if(channel !=null){
                    _channels.Remove(channel);
                }
            }
        }
        private void TryRemove(IChannel channel){
            lock(_lockObj){
                if(_channels.Contains(channel)){
                    _channels.Remove(channel);
                }
            }
        }
        private IChannel TryGetOneRandom(){
            IChannel channel = null ;
            lock(_lockObj){
                if(_channels.Count  ==0){
                    throw new Exceptions.RpcException("NettyRpcMultiplexContext wasn't inited");
                }
                int i = new Random().Next(10000);
                var index = Math.Abs( i  % _channels.Count) ; // 随机获取一个IChannel
                channel = _channels[index];
            }
            return channel;
        }

        private void StartConnect(EndPoint endpoint){
            int tryCount  = 0;
            Thread thread = new Thread(new ThreadStart(()=>{
                while(_autoReConnect){
                    tryCount++;
                    Logger.Debug("尝试在{0}秒后自动重连{1},尝试次数{2}",tryCount*5000,endpoint,tryCount);
                    Thread.Sleep(tryCount*5000);
                    try{
                        CreateConnection(endpoint,1).Wait();
                        break;
                    }
                    catch{
                        Logger.Error("自动重连{0}失败，尝试次数{1}",endpoint,tryCount);
                    }

                }

            }));
            thread.IsBackground = true;
            thread.Start();
        }
    }
}