using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.Netty
{
    public class NettyServerBootstrap<TMessage> : IServerBootstrap where TMessage : InvokeMessage
    {
        private readonly ILogger Logger;
        private readonly ILoggerFactory _factory;

        private IChannel _channel;
        private MultithreadEventLoopGroup _bossGroup;
        private MultithreadEventLoopGroup _workerGroup;

        private readonly IMessageCodecs<TMessage> _msgCodecs;
        private readonly IServerMessageHandler<TMessage> _handler;
        private readonly IContextAccessor<TMessage> _contextAccessor;

        public NettyServerBootstrap(IServerMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, ILoggerFactory factory) : this(handler, msgCodecs, factory, null)
        { }

        public NettyServerBootstrap(IServerMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, ILoggerFactory factory, IContextAccessor<TMessage> contextAccessor)
        {
            this._msgCodecs = msgCodecs;
            this._handler = handler;
            this._contextAccessor = contextAccessor;
            this.Logger = factory.CreateLogger(this.GetType());
            this._factory = factory;
        }

        public async Task ShutdownAsync()
        {
            if (_channel == null)
            {
                Logger.LogWarning("netty channel is null");
                return;
            }

            if (_channel != null)
            {
                Logger.LogInformation("dotbpe server stopping");
                await this._channel.CloseAsync();
                Logger.LogInformation("dotbpe server stoped");
            }
            if (_bossGroup != null && _workerGroup != null)
            {
                //NOTE: 如果取消注释这条，下面两条优雅推出会执行很慢，并超时，可以考虑更优的方法，实际不执行也没多大影响
                //await Task.WhenAll(_bossGroup.ShutdownGracefullyAsync(), _workerGroup.ShutdownGracefullyAsync());
                Logger.LogInformation("netty gourp shutdowned");
            }
        }

        /// <summary>
        /// 启动服务，
        /// </summary>
        /// <param name="endPoint">绑定到本地的服务地址</param>
        /// <returns></returns>
        public async Task StartAsync(EndPoint localPoint, CancellationToken token)
        {
            // 主的线程
            _bossGroup = new MultithreadEventLoopGroup(1);
            // 工作线程，默认根据CPU计算
            _workerGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap()
                .Group(_bossGroup, _workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 128); //NOTE: 是否可以公开更多Netty的参数

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }

            bootstrap.Handler(new LoggingHandler("LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(new LoggingHandler("CONN"));
                    MessageMeta meta = _msgCodecs.GetMessageMeta();

                    // IdleStateHandler
                    pipeline.AddLast("timeout", new IdleStateHandler(0, 0, meta.HeartbeatInterval / 1000 * 2)); //服务端双倍来处理

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
                    //收到消息后的解码处理Handler
                    pipeline.AddLast(new ChannelDecodeHandler<TMessage>(_msgCodecs));
                    //业务处理Handler，即解码成功后如何处理消息的类
                    pipeline.AddLast(new ServerChannelHandlerAdapter<TMessage>(this, this._factory));
                }));

            _channel = await bootstrap.BindAsync(localPoint);

            Logger.LogInformation("DotBPE RPC Server bind at {localPoint}", localPoint);
        }

        public async Task ChannelRead(IChannelHandlerContext ctx, TMessage message)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, _msgCodecs);
            context.LocalAddress = ctx.Channel.LocalAddress;
            context.RemoteAddress = ctx.Channel.RemoteAddress;

            CallContext<TMessage> callContext = null;
            if (_contextAccessor != null)
            {
                callContext = new CallContext<TMessage>(context);
                _contextAccessor.CallContext = callContext;
            }

            await this._handler.ReceiveAsync(context, message);

            if (callContext != null)
            {
                callContext.Dispose();
                callContext = null;
            }
            context = null;
        }
    }
}
