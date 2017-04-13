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
using DotBPE.Rpc.Exceptions;
using Microsoft.Extensions.Configuration;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultRpcClient<TMessage>:IRpcClient<TMessage> where TMessage :InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultRpcClient<TMessage>>();
        private readonly ITransportFactory<TMessage> _factory;

        private readonly IConfiguration _config;
        private EndPoint _defaultServerAddress = null;

        private readonly IMessageHandler<TMessage> _handler;

        public DefaultRpcClient(ITransportFactory<TMessage> factory,
            IConfiguration config,
            IMessageHandler<TMessage> handler
           )
        {
            this._factory = factory;
            this._config = config;
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
            return transport.SendAsync(message);
        }

        public Task SendAsync(TMessage message)
        {
            var remote = GetDefaultRemoteAddress();
            return this.SendAsync(remote, message);
        }

        private EndPoint GetDefaultRemoteAddress()
        {
            if (_defaultServerAddress == null)
            {
                string serverIp = this._config["serverIp"];
                int port = int.Parse(this._config["serverPort"]);
                if (string.IsNullOrEmpty(serverIp) || port <= 0)
                {
                    throw new RpcException("不存在默认的服务器地址");
                }
                _defaultServerAddress = new IPEndPoint(IPAddress.Parse(serverIp), port);
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