#region copyright
// -----------------------------------------------------------------------
//  <copyright file="NettyClientBootstrap.cs” project="DotBPE.Rpc.Netty">
//    文件说明:
//     copyright@2017 xuanye 2017-04-08 9:32
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Options;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotBPE.Rpc.Logging;
using DotNetty.Handlers.Timeout;
using Microsoft.Extensions.Options;

namespace DotBPE.Rpc.Netty
{
    public class NettyClientBootstrap<TMessage> : IClientBootstrap<TMessage> where TMessage : InvokeMessage
    {
        static ILogger Logger = Environment.Logger.ForType<NettyClientBootstrap<TMessage>>();
        private readonly Bootstrap _bootstrap;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly IMessageCodecs<TMessage> _msgCodecs;

        private readonly IOptions<RpcClientOption> _clientOption;

        public NettyClientBootstrap(IMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs, IOptions<RpcClientOption> option)
        {
            this._clientOption = option;

            _bootstrap = InitBootstrap();
            _handler = handler;
            _msgCodecs = msgCodecs;
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
                    pipeline.AddLast(new ClientChannelHandlerAdapter<TMessage>(this));

                }));
            return bootstrap;
        }

        public async Task<IRpcContext<TMessage>> ConnectAsync(EndPoint endpoint)
        {
            var context = new NettyRpcMultiplexContext<TMessage>(this._bootstrap, this._msgCodecs);
            await context.InitAsync(endpoint,_clientOption?.Value);
            context.BindDisconnect(this);
            return context;
        }

        public event EventHandler<DisConnectedArgs> DisConnected;


        public void OnChannelInactive(IChannelHandlerContext context)
        {
            var args = new DisConnectedArgs();
            args.EndPoint = context.Channel.RemoteAddress;
            args.ContextId  = context.Channel.Id.AsLongText();
            this.DisConnected?.Invoke(this,args);
        }

        public void ChannelRead(IChannelHandlerContext ctx, TMessage msg)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, this._msgCodecs);
            context.LocalAddress = Utils.ParseUtils.ParseEndPointToIPString(ctx.Channel.LocalAddress);
            context.RemoteAddress = Utils.ParseUtils.ParseEndPointToIPString(ctx.Channel.RemoteAddress);

            this._handler.ReceiveAsync(context, msg);
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

        }
    }
}
