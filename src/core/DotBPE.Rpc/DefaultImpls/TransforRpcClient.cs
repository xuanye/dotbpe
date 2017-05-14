using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    /// <summary>
    /// 本地转发客户端，即收到请求后，通过桥接路由器找到目标地址，并直接转发到目标服务器，一般用于Http接口程序做转发用
    /// </summary>
    public class TransforRpcClient<TMessage> : IRpcClient<TMessage> where TMessage:InvokeMessage
    {
       static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<TransforRpcClient<TMessage>>();
        private readonly ITransportFactory<TMessage> _transportFactory;
        private readonly IMessageHandler<TMessage> _handler;

        private readonly IBridgeRouter<TMessage> _router;

        public TransforRpcClient(ITransportFactory<TMessage> factory,
        IMessageHandler<TMessage> handler ,
        IBridgeRouter<TMessage> router
        ){
            this._transportFactory = factory;
            this._handler = handler;
            this._handler.Recieved += Message_Recieved;
            this._router = router;

        }
        private void Message_Recieved(object sender, MessageRecievedEventArgs<TMessage> args)
        {
            //只处理服务端返回的请求，而不处理客户端发送的请求
            if(args.Message.InvokeMessageType != InvokeMessageType.Request){
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
            var point =  _router.GetRouterPoint(message);
            if(point==null){
                Logger.Error("Get routing error");
                throw new RpcException("Get routing information error, please check the configuration");
            }
            if(point.RoutePointType == RoutePointType.Local){ //本地调用流程
                throw new RpcException("service not found");
            }
            else{
                Logger.Debug("Call  remote  service{0}",point.RemoteAddress);
                var transport = this._transportFactory.CreateTransport(point.RemoteAddress);
                return transport.SendAsync(message);
            }
            throw new NotImplementedException("There is no default address,call SendAsync(EndPoint serverAddress,AmpMessage message) to send messages");
        }
    }
}
