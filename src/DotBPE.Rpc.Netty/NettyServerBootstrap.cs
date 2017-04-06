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
        public NettyServerBootstrap(IMessageCodecs<IMessage> msgCodecs, ILogger logger)
        {
            this._logger = logger;
            this._msgCodecs = msgCodecs;
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
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));

               
                    pipeline.AddLast(new ChannelDecodeHandler<IMessage>(_msgCodecs));
                    
                    pipeline.AddLast(new ServerHandlerAdapter(async (ctx, message) =>
                    {
                        //这里的消息已经解码了，需要后处理，并产地给处理程序如何发送回复消息的接口
                        var sender = new NettyServerMessageSender<IMessage>(ctx, _msgCodecs);
                        await OnReceived(sender, message);
                    }, _logger));
                    

                }));

            this._channel = await bootstrap.BindAsync(endPoint);

          
            this._logger.LogDebug($"服务主机启动成功，监听地址：{endPoint}。");
        }
    }
}
