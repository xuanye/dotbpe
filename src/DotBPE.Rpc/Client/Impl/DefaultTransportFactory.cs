// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultTransportFactory : ITransportFactory
    {
        static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);


        private readonly ConcurrentDictionary<EndPoint, ITransport> _transportCache = new ConcurrentDictionary<EndPoint, ITransport>();
        private readonly ISocketClient<AmpMessage> _socket;
        private readonly IClientMessageHandler _handler;
        private readonly ILogger<DefaultTransportFactory> _logger;

        public DefaultTransportFactory(
            ISocketClient<AmpMessage> socket,
            IClientMessageHandler handler,
            ILogger<DefaultTransportFactory> logger
            )
        {
            _socket = socket;
            _handler = handler;
            _logger = logger;

            _socket.OnIdleState += Socket_OnIdleState;
            _socket.OnReceived += Socket_OnReceived;
            _socket.OnConnected += Socket_OnConnectedAsync;
            _socket.OnDisconnected += Socket_OnDisconnected;
            _socket.OnError += Socket_OnError;
        }

        public async Task CloseAllTransports(CancellationToken cancellationToken)
        {
            foreach (var transport in _transportCache.Values)
            {
                await transport.CloseAsync(CancellationToken.None).AnyContext();
            }
            _transportCache.Clear();
        }

        public async Task CloseTransportAsync(EndPoint endpoint)
        {
            _logger.LogInformation("close transport {0}", endpoint);
            if (_transportCache.TryRemove(endpoint, out var transport))
            {
                await transport.CloseAsync(CancellationToken.None).AnyContext();
                _logger.LogInformation("close transport {0},completed", endpoint);
            }
        }

        public async Task<ITransport> CreateTransport(EndPoint endpoint)
        {
            if (_transportCache.TryGetValue(endpoint, out var transport))
            {
                return transport;
            }

            await SemaphoreSlim.WaitAsync();
            try
            {
                if (_transportCache.TryGetValue(endpoint, out transport))
                {
                    return transport;
                }

                var context = await _socket.ConnectAsync(endpoint);
                transport = new DefaultTransport(context);
                _transportCache.AddOrUpdate(endpoint, transport, (k, old) =>
                {
                    //Dispose
                    old.CloseAsync(CancellationToken.None).AnyContext();
                    return transport;
                });

                return transport;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during create transport");
                throw;
            }
            finally

            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                SemaphoreSlim.Release();
            }
        }


        #region Socket Events
        /// <summary>
        /// heart beat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnIdleState(object sender, Peach.EventArgs.IdleStateEventArgs<AmpMessage> e)
        {
            _logger.LogTrace("send heart beat");
            e.Context.SendAsync(AmpMessage.CreateHeartBeatMessage(Codec.CodecType.Unknown));
        }

        /// <summary>
        /// on receive message ,raise message handler event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnReceived(object sender, Peach.EventArgs.MessageReceivedEventArgs<AmpMessage> e)
        {
            _logger.LogDebug("receive message ,id={0} ,code = {1}", e.Message.Id, e.Message.Code);
            _handler.RaiseReceive(e.Message);
        }



        private void Socket_OnError(object sender, Peach.EventArgs.ErrorEventArgs<AmpMessage> e)
        {
            _logger.LogError("-------------------client:error occ---------------------");
            _logger.LogError(e.Error, e.Error.Message);
        }

        private void Socket_OnDisconnected(object sender, Peach.EventArgs.DisconnectedEventArgs<AmpMessage> e)
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            CloseTransportAsync(ConvertIPV4EndPoint(e.Context.RemoteEndPoint)).AnyContext();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

            _logger.LogInformation("Client{Id}:Connection is being disconnected,remote address:{RemoteAddress}", e.Context.Id, e.Context.RemoteEndPoint);
        }

        private void Socket_OnConnectedAsync(object sender, Peach.EventArgs.ConnectedEventArgs<AmpMessage> e)
        {

            _logger.LogInformation("Client{Id}:Connection established,remote address:{RemoteAddress}", e.Context.Id, e.Context.RemoteEndPoint);
        }

        private EndPoint ConvertIPV4EndPoint(IPEndPoint endpoint)
        {
            return new IPEndPoint(endpoint.Address.MapToIPv4(), endpoint.Port);
        }
        #endregion
    }
}
