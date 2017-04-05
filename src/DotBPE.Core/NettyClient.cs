using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Core
{
    public class NettyClient:IDisposable
    {
        public NettyClient(string[] address,
            int connectTimeOut = 15000,
            int pingInterval = 60000,
            int maxPackageSie = 2000000,
            int connSizePerAddr =5,
            int timerInterval = 100,
            int reconnectInterval = 1,
            int startPort = -1
            )
        {
            this._address = address;
            this._startPort = startPort;
            this._connSizePerAddr = connSizePerAddr;
        }
        private int _startPort = -1;
        private readonly string[] _address = null;
        private readonly List<IChannel> channels = new List<IChannel>();
        private IEventLoopGroup eventLoopGroup = null;
        private int _connSizePerAddr = 1;

        private static IPAddress LocalAddress = IPAddress.Parse("127.0.0.1");
        public void Dispose()
        {
           
        }

        public void Init()
        {

        }

        public async void Start()
        {
            eventLoopGroup = new MultithreadEventLoopGroup();
            
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    // pipeline.AddLast(new LoggingHandler());
                    // pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    // pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    // pipeline.AddLast("echo", new EchoClientHandler());
                }));

            foreach(string addr in this._address)
            {
                string[] arr_adress = addr.Split(':');
                var ip = IPAddress.Parse(arr_adress[0]);
                var port = int.Parse(arr_adress[1]);

                var remote = new IPEndPoint(ip, port);
                for (int i = 0; i < this._connSizePerAddr; i++) {
                }
                IChannel channel = null;
                if (this._startPort > 0)
                {                      
                    var local = new IPEndPoint(LocalAddress, this._startPort++);
                    channel = await bootstrap.ConnectAsync(remote, local);
                    if(this._startPort >= IPEndPoint.MaxPort)
                    {
                        this._startPort = 1025;
                    }
                }
                else
                {
                    channel = await bootstrap.ConnectAsync(remote);
                }
                
                this.channels.Add(channel);
            }      
        }
    }
}