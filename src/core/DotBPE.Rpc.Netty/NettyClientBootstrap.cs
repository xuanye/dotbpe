#region copyright

// -----------------------------------------------------------------------
//  <copyright file="NettyClientBootstrap.cs” project="DotBPE.Rpc.Netty">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 9:32
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Options;
using DotBPE.Rpc.Utils;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Netty
{
    public class NettyClientBootstrap<TMessage> : IClientBootstrap<TMessage> where TMessage : InvokeMessage
    {
        private static ConcurrentDictionary<EndPoint, IRpcContext<TMessage>> CON_LIST = new ConcurrentDictionary<EndPoint, IRpcContext<TMessage>>();
        private readonly Bootstrap _bootstrap;
        private readonly IOptions<RpcClientOption> _clientOption;
        private readonly ILoggerFactory _factory;
        private readonly IClientMessageHandler<TMessage> _handler;
        private readonly IMessageCodecs<TMessage> _msgCodecs;
        private readonly IOptions<RemoteServicesOption> _serviceOption;
        private readonly ILogger Logger;
        private bool _stoped = false;

        public NettyClientBootstrap(
            IClientMessageHandler<TMessage> handler,
            IMessageCodecs<TMessage> msgCodecs,
            IOptions<RpcClientOption> option,
            ILoggerFactory factory,
            IOptions<RemoteServicesOption> serviceOption = null
            )
        {
            this._clientOption = option;

            _bootstrap = InitBootstrap();
            _handler = handler;
            _msgCodecs = msgCodecs;
            _serviceOption = serviceOption;
            this.Logger = factory.CreateLogger(this.GetType());
            this._factory = factory;
        }

        public event EventHandler<ConnectionEventArgs> DisConnected;

        public void ChannelRead(IChannelHandlerContext ctx, TMessage msg)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, this._msgCodecs);
            context.LocalAddress = ctx.Channel.LocalAddress;
            context.RemoteAddress = ctx.Channel.RemoteAddress;

            this._handler.Receive(context, msg);
        }

        public void Dispose()
        {
            //do nothing
        }

        public IRpcContext<TMessage> GetContext(EndPoint remotePoint)
        {
            IRpcContext<TMessage> ctx = null;
            if (CON_LIST.ContainsKey(remotePoint))
            {
                ctx = CON_LIST[remotePoint];
            }
            //if(_clientOption !=null &&  _clientOption.Value !=null && !string.IsNullOrEmpty(_clientOption.Value.DefaultServerAddress))
            {
                var remote = remotePoint; //ParseUtils.ParseEndPointFromString(_clientOption.Value.DefaultServerAddress);
                var context = new NettyRpcMultiplexContext<TMessage>(this._bootstrap, this._msgCodecs, this.Logger);
                context.InitAsync(remote, _clientOption?.Value).Wait() ;
                context.BindDisconnect(this);
                if (CON_LIST.TryAdd(remote, context))
                {
                    ctx = context;
                }                
            }
            return ctx;
        }

        public void OnChannelInactive(IChannelHandlerContext context)
        {
            var args = new ConnectionEventArgs
            {
                RemotePoint = context.Channel.RemoteAddress,
                LocalPoint = context.Channel.LocalAddress,
                ChannelId = context.Channel.Id.AsLongText()
            };

            this.DisConnected?.Invoke(this, args);
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task SendHeartbeatAsync(IChannelHandlerContext ctx, IdleStateEvent state)
        {
            //获取心跳包的打包内容
            TMessage message = this._msgCodecs.HeartbeatMessage();
            var heartbeatBuff = ctx.Allocator.Buffer(message.Length);
            var bufferWritter = NettyBufferManager.CreateBufferWriter(heartbeatBuff);
            this._msgCodecs.Encode(message, bufferWritter);

            return ctx.WriteAndFlushAsync(heartbeatBuff);
        }

        public Task StartAsync()
        {
            if (_serviceOption == null || _serviceOption.Value == null)
            {
                return Task.CompletedTask;
            }
            HashSet<string> remoteAddress = new HashSet<string>();

            var routeOptions = this._serviceOption.Value;
            foreach (var option in routeOptions)
            {
                string[] arrAdd = option.RemoteAddress.Split(',');
                foreach (string address in arrAdd)
                {
                    if (!remoteAddress.Contains(address))
                    {

                        remoteAddress.Add(address);
                    }
                }
            }
            if (remoteAddress.Count > 0)
            {
                int mcount = _clientOption?.Value.MultiplexCount > 0 ? _clientOption.Value.MultiplexCount : 1;
                foreach (var address in remoteAddress)
                {
                    StartConnect(address, mcount);
                }
            }
            return Task.CompletedTask;
        }


        public async Task StopAsync()
        {
            _stoped = true;
            if (CON_LIST.Count > 0)
            {
                foreach (var kv in CON_LIST)
                {
                    await kv.Value.CloseAsync();
                }
            }
            CON_LIST.Clear();
        }

        private Bootstrap InitBootstrap()
        {   
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3))                
                .Handler(new ActionChannelInitializer<ISocketChannel>(c =>
                {
                    var pipeline = c.Pipeline;
                    pipeline.AddLast(new LoggingHandler("CLT-CONN"));
                    MessageMeta meta = _msgCodecs.GetMessageMeta();

                    // IdleStateHandler
                    pipeline.AddLast("timeout", new IdleStateHandler(0, 0, meta.HeartbeatInterval / 1000));
                    //消息前处理
                    pipeline.AddLast(
                        new LengthFieldBasedFrameDecoder(
                            meta.MaxFrameLength,
                            meta.LengthFieldOffset,
                            meta.LengthFieldLength,
                            meta.LengthAdjustment,
                            meta.InitialBytesToStrip
                        )
                    );

                    pipeline.AddLast(new ChannelDecodeHandler<TMessage>(_msgCodecs));
                    pipeline.AddLast(new ClientChannelHandlerAdapter<TMessage>(this, this._factory));
                }));
            return bootstrap;
        }

        private Task StartConnect(string address, int mCount)
        {
            int tryCount = 0;
            var remote = ParseUtils.ParseEndPointFromString(address);
            return Task.Factory.StartNew(async () =>
            {
                while (!_stoped)
                {

                    tryCount++;
                    if (tryCount >= 100000)
                    {
                        tryCount = 1;
                        Logger.LogWarning("reconnect to {0} 100000 times, but fail and restart !", address);                      
                    }
                    
                    try
                    {
                        var context = new NettyRpcMultiplexContext<TMessage>(this._bootstrap, this._msgCodecs, this.Logger);
                        await context.InitAsync(remote, _clientOption?.Value);
                        context.BindDisconnect(this);
                        if (CON_LIST.TryAdd(remote, context))
                        {
                            break;
                        }
                        else
                        {
                            await context.CloseAsync();
                        }                       
                    }
                    catch
                    {
                        Logger.LogWarning("reconnect {0} failed，try {1} times", address, tryCount);
                    }
                    Logger.LogInformation("will reconnect to {0} after {1} ms, try {2} times", address, tryCount * 1000, tryCount);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                  
                }
            });
        }
    }
}
