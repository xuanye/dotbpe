using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.Netty {
    public class NettyServerBootstrap<TMessage> : IServerBootstrap where TMessage : InvokeMessage {
        private readonly ILogger Logger;
        private readonly ILoggerFactory _factory;
        private IChannel _channel;
        private readonly IMessageCodecs<TMessage> _msgCodecs;
        private readonly IServerMessageHandler<TMessage> _handler;

        private readonly IContextAccessor<TMessage> _contextAccessor;

        public NettyServerBootstrap (IServerMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, ILoggerFactory factory) : this (handler, msgCodecs, factory, null) { }

        public NettyServerBootstrap (IServerMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, ILoggerFactory factory, IContextAccessor<TMessage> contextAccessor) {
            this._msgCodecs = msgCodecs;
            this._handler = handler;
            this._contextAccessor = contextAccessor;
            this.Logger = factory.CreateLogger (this.GetType ());
            this._factory = factory;
        }

        public void Dispose () {
            if (_channel == null) {
                return;
            }
            if (this._channel.Open || this._channel.Active) {
                this._channel.CloseAsync ();
                this._channel = null;
            }
        }

        public Task ShutdownAsync () {
            if (_channel == null) {
                return Task.CompletedTask;
            }
            if (this._channel.Open || this._channel.Active) {
                this._channel.CloseAsync ();
                this._channel = null;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动服务，
        /// </summary>
        /// <param name="endPoint">绑定到本地的服务地址</param>
        /// <returns></returns>
        public async Task StartAsync (EndPoint localPoint) {
            // 主的线程
            var bossGroup = new MultithreadEventLoopGroup (1);
            // 工作线程，默认根据CPU计算
            var workerGroup = new MultithreadEventLoopGroup ();

            var bootstrap = new ServerBootstrap ()
                .Group (bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel> ()
                .Option (ChannelOption.SoBacklog, 128) //NOTE: 是否可以公开更多Netty的参数
                .Handler (new LoggingHandler ("SRV-LSTN"))
                .ChildHandler (new ActionChannelInitializer<ISocketChannel> (channel => {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast (new LoggingHandler ("SRV-CONN"));
                    MessageMeta meta = _msgCodecs.GetMessageMeta ();

                    // IdleStateHandler
                    pipeline.AddLast ("timeout", new IdleStateHandler (0, 0, meta.HeartbeatInterval / 1000 * 2)); //服务端双倍来处理

                    //消息前处理
                    pipeline.AddLast (
                        new LengthFieldBasedFrameDecoder (
                            meta.MaxFrameLength,
                            meta.LengthFieldOffset,
                            meta.LengthFieldLength,
                            meta.LengthAdjustment,
                            meta.InitialBytesToStrip
                        )
                    );
                    //收到消息后的解码处理Handler
                    pipeline.AddLast (new ChannelDecodeHandler<TMessage> (_msgCodecs));

                    //业务处理Handler，即解码成功后如何处理消息的类
                    pipeline.AddLast (new ServerChannelHandlerAdapter<TMessage> (this, this._factory));
                }));

            this._channel = await bootstrap.BindAsync (localPoint);
        }

        public async Task ChannelRead (IChannelHandlerContext ctx, TMessage message) {
            var context = new NettyRpcContext<TMessage> (ctx.Channel, _msgCodecs);
            context.LocalAddress = ctx.Channel.LocalAddress;
            context.RemoteAddress = ctx.Channel.RemoteAddress;

            CallContext<TMessage> callContext = null;
            if (_contextAccessor != null) {
                callContext = new CallContext<TMessage> (context);
                _contextAccessor.CallContext = callContext;
            }

            await this._handler.ReceiveAsync (context, message);

            if (callContext != null) {
                callContext.Dispose ();
                callContext = null;
            }
            context = null;
        }
    }
}