#region copyright

// -----------------------------------------------------------------------
//  <copyright file="NettyClientBootstrap.cs” project="DotBPE.Rpc.Netty">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 9:32
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Options;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Netty
{
    public class NettyClientBootstrap<TMessage> : IClientBootstrap<TMessage> where TMessage : InvokeMessage
    {
        private readonly ILogger Logger;
        private readonly ILoggerFactory _factory;
        private readonly Bootstrap _bootstrap;
        private readonly IClientMessageHandler<TMessage> _handler;
        private readonly IMessageCodecs<TMessage> _msgCodecs;

        private readonly IOptions<RpcClientOption> _clientOption;

        public NettyClientBootstrap(IClientMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, IOptions<RpcClientOption> option, ILoggerFactory factory)
        {
            this._clientOption = option;

            _bootstrap = InitBootstrap();
            _handler = handler;
            _msgCodecs = msgCodecs;

            this.Logger = factory.CreateLogger(this.GetType());
            this._factory = factory;
        }

        private Bootstrap InitBootstrap()
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3))
                .Group(new MultithreadEventLoopGroup())
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

        public async Task<IRpcContext<TMessage>> StartConnectAsync(EndPoint remoteAddress)
        {
            var context = new NettyRpcMultiplexContext<TMessage>(this._bootstrap, this._msgCodecs, this.Logger);
            await context.InitAsync(remoteAddress, _clientOption?.Value);
            context.BindDisconnect(this);
            return context;
        }

        public event EventHandler<ConnectionEventArgs> DisConnected;

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

        public void ChannelRead(IChannelHandlerContext ctx, TMessage msg)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, this._msgCodecs);
            context.LocalAddress = ctx.Channel.LocalAddress;
            context.RemoteAddress = ctx.Channel.RemoteAddress;

            this._handler.Receive(context, msg);
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

        public void Dispose()
        {
            //do nothing
        }
    }
}
