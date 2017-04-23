#region copyright
// -----------------------------------------------------------------------
//  <copyright file="RpcClient.cs” project="DotBPE.Rpc">
//    文件说明:
//     copyright@2017 xuanye 2017-04-09 10:50
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Options;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;
using Microsoft.Extensions.Options;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultRpcClient<TMessage>:IRpcClient<TMessage> where TMessage :InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultRpcClient<TMessage>>();
        private readonly ITransportFactory<TMessage> _factory;

        private EndPoint _defaultServerAddress = null;
        private IOptions<RpcClientOption> _clientOption;
        private readonly IMessageHandler<TMessage> _handler;

        public DefaultRpcClient(ITransportFactory<TMessage> factory,
            IOptions<RpcClientOption> clientOption,
            IMessageHandler<TMessage> handler
           )
        {
            this._clientOption = clientOption;
            this._factory = factory;
            this._handler = handler;
            this._handler.Recieved += Message_Recieved;
        }

        private void Message_Recieved(object sender, MessageRecievedEventArgs<TMessage> args)
        {
            Recieved?.Invoke(sender, args);
        }

        public event EventHandler<MessageRecievedEventArgs<TMessage>> Recieved;
        public Task SendAsync(EndPoint serverAddress, TMessage message)
        {
            var transport = this._factory.CreateTransport(serverAddress);
            Logger.Debug("使用Transport={0}发送消息",transport.Id);
            return transport.SendAsync(message);
        }

        public Task SendAsync(TMessage message)
        {
            var remote = GetDefaultRemoteAddress();
            Logger.Debug("远端地址:{0}",remote);
            return this.SendAsync(remote, message);
        }

        private EndPoint GetDefaultRemoteAddress()
        {
            if (_defaultServerAddress == null)
            {

                if(_clientOption ==null){
                    throw new RpcException("不存在默认的服务器地址");
                }
                string serverAddress = _clientOption.Value.DefaultServerAddress;
                if (string.IsNullOrEmpty(serverAddress))
                {
                    throw new RpcException("不存在默认的服务器地址");
                }

                _defaultServerAddress = Utils.ParseUtils.ParseEndPointFromString(serverAddress);
            }
            return _defaultServerAddress;
        }

        public Task CloseAsync()
        {
            var remote = GetDefaultRemoteAddress();
            return CloseAsync(remote);
        }

        public Task CloseAsync(EndPoint serverAddress)
        {
            return this._factory.CloseTransportAsync(serverAddress);
        }
    }
}