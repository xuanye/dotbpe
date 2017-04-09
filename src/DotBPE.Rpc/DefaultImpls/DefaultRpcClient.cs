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
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultRpcClient<TMessage>:IRpcClient<TMessage> where TMessage :InvokeMessage
    {
        private readonly ITransportFactory<TMessage> _factory;
        private readonly ILogger<DefaultRpcClient<TMessage>> _logger;
        private readonly IConfiguration _config;
        private EndPoint _defaultServerAddress = null;
        public DefaultRpcClient(ITransportFactory<TMessage> factory,
            IConfiguration config,
            ILogger<DefaultRpcClient<TMessage>> logger
           )
        {
            this._factory = factory;
            this._logger = logger;
            this._config = config;
          
        }

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

        public event EventHandler<TMessage> Receieved;

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
    }
}