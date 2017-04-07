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

namespace DotBPE.Rpc.Netty
{
    public class NettyServerBootstrap : IServerBootstrap
    {
        private readonly ILogger _logger;
        private IChannel _channel;
        private readonly IMessageCodecs<IMessage> _msgCodecs;
        private readonly IMessageHandler<IMessage> _handler;
        public NettyServerBootstrap(IMessageHandler<IMessage> handler, IMessageCodecs<IMessage> msgCodecs, ILogger logger)
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
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

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

                    pipeline.AddLast(new ChannelDecodeHandler<IMessage>(_msgCodecs)); 
                    pipeline.AddLast(new ServerHandlerAdapter(async (ctx, message) =>
                    {
                        //这里的消息已经解码了，需要后处理，并产地给处理程序如何发送回复消息的接口
                        var context = new NettyRpcContext<IMessage>(ctx, _msgCodecs);

                        // 这里添加实际的消息处理程序
                        await this._handler.RecieveAsync(context, message);
                    }, _logger));

                }));

            this._channel = await bootstrap.BindAsync(endPoint);

          
            this._logger.LogDebug($"服务主机启动成功，监听地址：{endPoint}。");
        }
    }
}
