using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Peach.Config;
using Peach.Infrastructure;

namespace Peach.Tcp
{
    /// <summary>
    /// Tcp Server
    /// </summary>
    public class TcpServerBootstrap<TMessage> : IServerBootstrap where TMessage : Messaging.IMessage
    {
        private readonly TcpHostOption _options;
        private readonly Protocol.IProtocol<TMessage> _protocol;
        private readonly ISocketService<TMessage> _socketService;
        private readonly ILogger _logger;

        private IChannel _channel;
        private MultithreadEventLoopGroup _bossGroup;
        private MultithreadEventLoopGroup _workerGroup;

        public TcpServerBootstrap(
                ISocketService<TMessage> socketService,
                Protocol.IProtocol<TMessage> protocol,
                ILoggerFactory loggerFactory,
                IOptions<TcpHostOption> hostOption = null
            )
        {

            Preconditions.CheckNotNull(protocol, nameof(protocol));
            Preconditions.CheckNotNull(socketService, nameof(socketService));

            _options = hostOption?.Value ?? new TcpHostOption();
            _protocol = protocol;
            _socketService = socketService;
            _logger = loggerFactory.CreateLogger(GetType());
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 主的线程
            _bossGroup = new MultithreadEventLoopGroup(1);
            // 工作线程，默认根据CPU计算
            _workerGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap()
                .Group(_bossGroup, _workerGroup);


            if (_options.UseLibuv)
            {
                bootstrap.Channel<TcpServerChannel>();
            }
            else
            {
                bootstrap.Channel<TcpServerSocketChannel>();
            }

            bootstrap.Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, _options.SoBacklog); //NOTE: 是否可以公开更多Netty的参数

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

                    //TODO:ssl support

                    pipeline.AddLast(new LoggingHandler("CONN"));
                    var meta = _protocol.GetProtocolMeta();

                    if (meta != null)
                    {
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
                    }
                    else //Simple Protocol For Test
                    {
                        pipeline.AddLast("timeout", new IdleStateHandler(0, 0, 360)); // heartbeat each 6 second
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                    }

                    //收到消息后的解码处理Handler
                    pipeline.AddLast(new ChannelDecodeHandler<TMessage>(_protocol));
                    //业务处理Handler，即解码成功后如何处理消息的类
                    pipeline.AddLast(new TcpServerChannelHandlerAdapter<TMessage>(_socketService,_protocol));
                }));

            if (_options.BindType == AddressBindType.Any)
            {
                _channel = await bootstrap.BindAsync(_options.Port);
            }
            else if (_options.BindType == AddressBindType.InternalAddress)
            {
                var localPoint = IPUtility.GetLocalIntranetIP();
                //this._logger.LogInformation("TcpServerHost bind at {0}",localPoint);
                _channel = await bootstrap.BindAsync(localPoint, _options.Port);
            }
            else if(_options.BindType == AddressBindType.Loopback)
            {
                _channel = await bootstrap.BindAsync(IPAddress.Loopback, _options.Port);
            }
            else
            {
                _channel = await bootstrap.BindAsync(IPAddress.Parse(_options.SpecialAddress), _options.Port);
            }

            _logger.LogInformation("TcpServerHost bind at {0}", _channel.LocalAddress);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TcpServerHost is stoping...");
            await _channel.CloseAsync();
            var quietPeriod = TimeSpan.FromMilliseconds(_options.QuietPeriod);
            var shutdownTimeout = TimeSpan.FromMilliseconds(_options.ShutdownTimeout);
            await _workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            await _bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            _logger.LogInformation("TcpServerHost is stoped!");
            //TODO:Close Client?
        }
    }
}
