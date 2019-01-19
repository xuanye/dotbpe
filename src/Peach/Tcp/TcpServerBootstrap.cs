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
    /// <inheritdoc />
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

            this._options = hostOption?.Value ?? new TcpHostOption();
            this._protocol = protocol;
            this._socketService = socketService;
            this._logger = loggerFactory.CreateLogger(GetType());
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 主的线程
            this._bossGroup = new MultithreadEventLoopGroup(1);
            // 工作线程，默认根据CPU计算
            this._workerGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap()
                .Group(this._bossGroup, this._workerGroup);


            if (this._options.UseLibuv)
            {
                bootstrap.Channel<TcpServerChannel>();
            }
            else
            {
                bootstrap.Channel<TcpServerSocketChannel>();
            }

            bootstrap.Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, this._options.SoBacklog); //NOTE: 是否可以公开更多Netty的参数

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
                    var meta = this._protocol.GetProtocolMeta();

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
                    pipeline.AddLast(new ChannelDecodeHandler<TMessage>(this._protocol));
                    //业务处理Handler，即解码成功后如何处理消息的类
                    pipeline.AddLast(new TcpServerChannelHandlerAdapter<TMessage>(this._socketService, this._protocol));
                }));

            if (this._options.BindType == AddressBindType.Any)
            {
                this._channel = await bootstrap.BindAsync(this._options.Port);
            }
            else if (this._options.BindType == AddressBindType.InternalAddress)
            {
                var localPoint = IPUtility.GetLocalIntranetIP();
                //this._logger.LogInformation("TcpServerHost bind at {0}",localPoint);
                this._channel = await bootstrap.BindAsync(localPoint, this._options.Port);
            }
            else if(this._options.BindType == AddressBindType.Loopback)
            {
                this._channel = await bootstrap.BindAsync(IPAddress.Loopback, this._options.Port);
            }
            else
            {
                this._channel = await bootstrap.BindAsync(IPAddress.Parse(this._options.SpecialAddress), this._options.Port);
            }

            Console.Write(this._options.StartupWords, this._channel.LocalAddress);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("TcpServerHost is stoping...");
            await this._channel.CloseAsync();
            var quietPeriod = TimeSpan.FromMilliseconds(this._options.QuietPeriod);
            var shutdownTimeout = TimeSpan.FromMilliseconds(this._options.ShutdownTimeout);
            await this._workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            await this._bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            this._logger.LogInformation("TcpServerHost is stoped!");
            //NOTE:Close Client?
        }
    }
}
