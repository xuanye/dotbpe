using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using Microsoft.Extensions.Logging.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DotBPE.Rpc.Netty
{
    public class NettyServerBootstrap<TMessage> : IServerBootstrap where TMessage :IMessage
    {
        private readonly ILogger<NettyServerBootstrap<TMessage>> _logger;
        private IChannel _channel;
        private readonly IMessageCodecs<TMessage> _msgCodecs;
        private readonly IMessageHandler<TMessage> _handler;
        public NettyServerBootstrap(IMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, ILogger<NettyServerBootstrap<TMessage>> logger)
        {
            this._logger = logger;
            this._msgCodecs = msgCodecs;
            this._handler = handler;
        }

        public void Dispose()
        {
            if(this._channel.Open || this._channel.Active)
            {
                Task task = this._channel.CloseAsync();
                task.Wait();
                this._channel = null;
            }           
        }

        public async Task StartAsync(EndPoint endPoint)
        {
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    MessageMeta meta = _msgCodecs.GetMessageMeta();
                  

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
                    pipeline.AddLast(new ServerChannelHandlerAdapter<TMessage>(this, _logger));

                }));

            this._channel = await bootstrap.BindAsync(endPoint);

          
            this._logger.LogDebug($"服务主机启动成功，监听地址：{endPoint}。");
        }

        public void ChannelRead(IChannelHandlerContext ctx, TMessage message)
        {
            var context = new NettyRpcContext<TMessage>(ctx, _msgCodecs);

            // 这里添加实际的消息处理程序
            this._handler.ReceiveAsync(context, message);
        }
    }
}
