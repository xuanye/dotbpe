using DotBPE.Rpc.Codes;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.DefaultImpls
{
    public class BridgeRpcClient<TMessage> : IRpcClient<TMessage> where TMessage : InvokeMessage
    {
        private readonly ILogger<BridgeRpcClient<TMessage>> Logger;
        private readonly ITransportFactory<TMessage> _transportFactory;
        private readonly IMessageHandler<TMessage> _handler;

        private readonly IBridgeRouter<TMessage> _router;

        private readonly IServiceActorLocator<TMessage> _actorLocator;

        public BridgeRpcClient(ITransportFactory<TMessage> factory,
            IMessageHandler<TMessage> handler,
            IBridgeRouter<TMessage> router,
            IServiceActorLocator<TMessage> actorLocator,
            ILogger<BridgeRpcClient<TMessage>> logger
        )
        {
            this.Logger = logger;
            this._transportFactory = factory;
            this._handler = handler;
            this._handler.Recieved += Message_Recieved;
            this._router = router;
            this._actorLocator = actorLocator;
        }

        private void Message_Recieved(object sender, MessageRecievedEventArgs<TMessage> args)
        {
            //只处理服务端返回的请求，而不处理客户端发送的请求
            if (args.Message.InvokeMessageType != InvokeMessageType.Request)
            {
                Recieved?.Invoke(sender, args);
            }
        }

        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

        public Task CloseAsync(EndPoint serverAddress)
        {
            return _transportFactory.CloseTransportAsync(serverAddress);
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException("There is no default address, call CloseAsync(EndPoint serverAddress) to close the connection");
        }

        /// <summary>
        /// 强制发送到目标地址
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendAsync(EndPoint serverAddress, TMessage message)
        {
            var transport = this._transportFactory.CreateTransport(serverAddress);
            return transport.SendAsync(message);
        }

        public Task SendAsync(TMessage message)
        {
            //根据配置 查找对应位置 来获得Endpoint
            var point = _router.GetRouterPoint(message);
            if (point == null)
            {
                Logger.LogError("Get routing error");
                throw new Rpc.Exceptions.RpcException("Get routing information error, please check the configuration");
            }
            if (point.RoutePointType == RoutePointType.Local)
            { //本地调用流程
                Logger.LogDebug("Call  local  service");
                var actor = this._actorLocator.LocateServiceActor(message);
                var context = new LocalMockContext<TMessage>(this._handler); //MOCK Context
                return actor.ReceiveAsync(context, message);
            }
            else
            {
                Logger.LogDebug("Call  remote  service{0}", point.RemoteAddress);
                var transport = this._transportFactory.CreateTransport(point.RemoteAddress);
                return transport.SendAsync(message);
            }
            throw new NotImplementedException("There is no default address,call SendAsync(EndPoint serverAddress,AmpMessage message) to send messages");
        }

        public void Dispose()
        {
            //释放所有链接
            this._transportFactory.Dispose();
        }
    }
}
