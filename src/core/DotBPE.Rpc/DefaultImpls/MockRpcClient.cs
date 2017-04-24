using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class MockRpcClient<TMessage> : IRpcClient<TMessage> where TMessage : InvokeMessage
    {
        static readonly ILogger Logger = Rpc.Environment.Logger.ForType<MockRpcClient<TMessage>>();
        private readonly IMessageHandler<TMessage> _handler;
        private readonly  IServiceActorLocator<TMessage> _actorLocator;
        public MockRpcClient(
        IMessageHandler<TMessage> handler ,
        IServiceActorLocator<TMessage> actorLocator
        ){
            this._handler = handler;
            this._handler.Recieved += Message_Recieved;
            this._actorLocator = actorLocator;
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
           return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
           throw new NotImplementedException("不存在默认地址，请使用CloseAsync(EndPoint serverAddress)关闭连接");
        }

        /// <summary>
        /// 强制发送到目标地址
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendAsync(EndPoint serverAddress, TMessage message)
        {
           throw new NotImplementedException("不存在默认地址，请使用CloseAsync(EndPoint serverAddress)关闭连接");
        }

        public Task SendAsync(TMessage message)
        {
            var actor= this._actorLocator.LocateServiceActor(message);
            var context = new LocalMockContext<TMessage>(this._handler); //MOCK Context
            return actor.ReceiveAsync(context,message);
        }
    }
}
