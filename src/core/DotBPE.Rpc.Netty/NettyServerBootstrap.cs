using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;

namespace DotBPE.Rpc.Netty
{
    public class NettyServerBootstrap<TMessage> : IServerBootstrap where TMessage :InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<NettyServerBootstrap<TMessage>>();
        private IChannel _channel;
        private readonly IMessageCodecs<TMessage> _msgCodecs;
        private readonly IMessageHandler<TMessage> _handler;

        private readonly IContextAccessor<TMessage> _contextAccessor;

         public NettyServerBootstrap(IMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs):this(handler,msgCodecs,null)
         {

         }
        public NettyServerBootstrap(IMessageHandler<TMessage> handler, IMessageCodecs<TMessage> msgCodecs,IContextAccessor<TMessage> contextAccessor)
        {
            this._msgCodecs = msgCodecs;
            this._handler = handler;
            this._contextAccessor = contextAccessor;
        }

        public void Dispose()
        {
            if(this._channel.Open || this._channel.Active)
            {
                this._channel.CloseAsync().Wait();
                this._channel = null;
            }
        }
        public Task ShutdownAsync()
        {
            return this._channel.CloseAsync();
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
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    MessageMeta meta = _msgCodecs.GetMessageMeta();

                    // IdleStateHandler
                    pipeline.AddLast("timeout", new IdleStateHandler(0,0,meta.HeartbeatInterval/1000*2)); //服务端双倍来处理

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
                    pipeline.AddLast(new ServerChannelHandlerAdapter<TMessage>(this));

                }));

            this._channel = await bootstrap.BindAsync(endPoint);

        }

        public async Task ChannelRead(IChannelHandlerContext ctx, TMessage message)
        {
            var context = new NettyRpcContext<TMessage>(ctx.Channel, _msgCodecs);
            context.LocalAddress = Utils.ParseUtils.ParseEndPointToIPString(ctx.Channel.LocalAddress);
            context.RemoteAddress = Utils.ParseUtils.ParseEndPointToIPString(ctx.Channel.RemoteAddress);

            CallContext<TMessage> callContext = null;
            if(_contextAccessor !=null){
                callContext = new CallContext<TMessage>(context);
                _contextAccessor.CallContext = callContext;
            }

            await this._handler.ReceiveAsync(context, message);

            if( callContext !=null)
            {
                callContext.Dispose();
                callContext = null; 
            }
            context =null;
        }


    }
}
