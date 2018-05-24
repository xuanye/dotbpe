using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// 默认的RpcClient实现，自动发现服务是否在远端注册，如果注册则优先调用远端服务，否则再在本地查找
    /// </summary>
    public class DefaultRpcClient<TMessage> : IRpcClient<TMessage> where TMessage : InvokeMessage
    {
        private readonly ITransportFactory<TMessage> _factory;
        private readonly ILogger<DefaultRpcClient<TMessage>> Logger;
        private readonly IClientMessageHandler<TMessage> _handler;
        private readonly IServiceActorLocator<TMessage> _actorLocator;

        private readonly IRouter<TMessage> _router;

        public DefaultRpcClient(
            ITransportFactory<TMessage> factory,
            IRouter<TMessage> router,
            IClientMessageHandler<TMessage> handler,
            IServiceActorLocator<TMessage> actorLocator,
            ILogger<DefaultRpcClient<TMessage>> logger
        )
        {
            //注册消息拦截器 收到消息时的处理函数
            this._router = router;
            this._handler = handler;
            this._factory = factory;
            logger.LogDebug("注册客户端拦截器消息");
            this._handler.Recieved -= Message_Recieved;
            this._handler.Recieved += Message_Recieved;

            this._actorLocator = actorLocator;
            this.Logger = logger;
        }

        private void Message_Recieved(object sender, MessageRecievedEventArgs<TMessage> args)
        {
            // 只处理服务端返回的请求，而不处理客户端发送的请求
            // 收到了消息然后冒泡到 上层
            if (args.Message.InvokeMessageType != InvokeMessageType.Request)
            {
                Logger.LogDebug("收到消息返回");

                Recieved?.Invoke(sender, args);
            }
        }

        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;

        public Task CloseAsync(EndPoint serverAddress)
        {
            return _factory.CloseTransportAsync(serverAddress);
        }

        /// <summary>
        /// 强制发送到目标地址
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendAsync(EndPoint serverAddress, TMessage message)
        {
            var transport = this._factory.CreateTransport(serverAddress);
            if (transport == null)
            {
                throw new Exceptions.RpcException("ITransport 不存在,或地址错误");
            }
            Logger.LogDebug("Transport:{transportId} send msg", transport.Id);
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
            {
                //本地调用流程
                Logger.LogDebug("Call  local  service");
                var actor = this._actorLocator.LocateServiceActor(message);
                if (actor == null)
                {
                    throw new Exceptions.RpcException($"ServiceActor 不存在，未配置对应的服务地址,MethodIdentifier={message.MethodIdentifier}");
                }
                var context = new LocalMockContext<TMessage>(this._handler); //MOCK Context
                return actor.ReceiveAsync(context, message);
            }
            else
            {
                Logger.LogDebug("Call  remote  service, {0}", point.RemoteAddress);
                var transport = this._factory.CreateTransport(point.RemoteAddress);
                if (transport == null)
                {
                    throw new Exceptions.RpcException("ITransport 不存在,或地址错误");
                }
                return transport.SendAsync(message);
            }
            throw new NotImplementedException("There is no default address,call SendAsync(EndPoint serverAddress,AmpMessage message) to send messages");
        }

        public void Dispose()
        {
            this._handler.Recieved -= Message_Recieved;

            //释放所有链接
            this._factory.Dispose();
        }
    }
}
