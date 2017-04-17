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
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotBPE.Rpc.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Buffers;

namespace DotBPE.Rpc.Netty
{
    public class NettyClientBootstrap<TMessage>:IClientBootstrap<TMessage> where TMessage:InvokeMessage
    {
        static ILogger Logger = Environment.Logger.ForType<NettyClientBootstrap<TMessage>>();
        private readonly Bootstrap _bootstrap;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly IMessageCodecs<TMessage> _msgCodecs;

        private static IByteBuffer _heartbeatBuff =null;
        public NettyClientBootstrap(IMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs)
        {

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
                    pipeline.AddLast("timeout", new IdleStateHandler(0,0,meta.PingInterval/1000));
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
            Logger.Debug("开始创建链接{0}",endpoint);
            var channel =  await this._bootstrap.ConnectAsync(endpoint);
            Logger.Debug("成功创建链接{0}",endpoint);
            return new NettyRpcContext<TMessage>(channel, this._msgCodecs);
        }

        public event EventHandler<EndPoint> Disconnected;


        public void OnChannelInactive(IChannelHandlerContext context)
        {
            this.Disconnected?.Invoke(this, context.Channel.RemoteAddress);
        }

        public void ChannelRead(IChannelHandlerContext ctx, TMessage msg)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, this._msgCodecs);
            this._handler.ReceiveAsync(context, msg);
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task SendHeartbeatAsync(IChannelHandlerContext ctx,IdleStateEvent state){
            //获取心跳包的打包内容
            if(_heartbeatBuff == null){
               TMessage message = this._msgCodecs.HeartbeatMessage();
               _heartbeatBuff  = Unpooled.Buffer(message.Length);
               var bufferWritter = NettyBufferManager.CreateBufferWriter(_heartbeatBuff);
               this._msgCodecs.Encode(message,bufferWritter);
            }
            return ctx.WriteAsync(_heartbeatBuff);
        }
        public void Dispose()
        {

        }
    }
}