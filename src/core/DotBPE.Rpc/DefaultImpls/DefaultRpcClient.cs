#region copyright

// -----------------------------------------------------------------------
//  <copyright file="RpcClient.cs” project="DotBPE.Rpc">
//    文件说明:
//     copyright@2017 xuanye 2017-04-09 10:50
//  </copyright>
// -----------------------------------------------------------------------

#endregion copyright

using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Options;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultRpcClient<TMessage> : IRpcClient<TMessage> where TMessage : InvokeMessage
    {
        private static readonly ILogger Logger = Environment.Logger.ForType<DefaultRpcClient<TMessage>>();
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
            Logger.Debug("Transport={0} send msg", transport.Id);
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
                if (_clientOption == null)
                {
                    throw new RpcException("no default server address");
                }
                string serverAddress = _clientOption.Value.DefaultServerAddress;
                if (string.IsNullOrEmpty(serverAddress))
                {
                    throw new RpcException("no default server address");
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

        public void Dispose()
        {
            // 释放链接
            this._factory?.Dispose();
        }
    }
}
