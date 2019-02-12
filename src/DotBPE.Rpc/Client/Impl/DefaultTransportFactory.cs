using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultTransportFactory : ITransportFactory<AmpMessage>
    {
        readonly ConcurrentDictionary<EndPoint, ITransport<AmpMessage>> TRANSPORT_CACHE = new ConcurrentDictionary<EndPoint, ITransport<AmpMessage>>();

        private readonly ISocketClient<AmpMessage> _socket;
        private readonly ISerializer _serializer;
        private readonly IClientMessageHandler<AmpMessage> _handler;
        private readonly ILogger<DefaultTransportFactory> _logger;

        static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public DefaultTransportFactory(
            ISocketClient<AmpMessage> socket,
            ISerializer serializer,
            IClientMessageHandler<AmpMessage> handler,
            ILogger<DefaultTransportFactory> logger
            )
        {
            this._socket = socket;
            this._serializer = serializer;
            this._handler = handler;
            this._logger = logger;
            this._socket.OnIdleState += _socket_OnIdleState;
            this._socket.OnReceived += _socket_OnReceived;
            this._socket.OnConnected += _socket_OnConnectedAsync;
            this._socket.OnDisconnected += _socket_OnDisconnected;
            this._socket.OnError += _socket_OnError;
        }



        #region Socket Events
        /// <summary>
        /// heart beat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _socket_OnIdleState(object sender, Peach.EventArgs.IdleStateEventArgs<AmpMessage> e)
        {
            this._logger.LogTrace("send heart beat");
            e.Context.SendAsync(AmpMessage.HEART_BEAT);
        }

        /// <summary>
        /// on receive message ,raise message handler event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _socket_OnReceived(object sender, Peach.EventArgs.MessageReceivedEventArgs<AmpMessage> e)
        {

            this._logger.LogDebug("receive message ,id={0} ,code = {1}",e.Message.Id,e.Message.Code);
            this._handler.RaiseReceive(e.Message);
        }



        private void _socket_OnError(object sender, Peach.EventArgs.ErrorEventArgs<AmpMessage> e)
        {
            this._logger.LogError("-------------------client:error occ---------------------");
            this._logger.LogError(e.Error,e.Error.Message);
        }

        private void _socket_OnDisconnected(object sender, Peach.EventArgs.DisconnectedEventArgs<AmpMessage> e)
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            CloseTransportAsync(ConvertIPV4EndPoint(e.Context.RemoteEndPoint)).AnyContext();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

            this._logger.LogInformation("-------------------client:connection is disconnected---------------------");
            this._logger.LogInformation(e.Context.Id);
        }

        private void _socket_OnConnectedAsync(object sender, Peach.EventArgs.ConnectedEventArgs<AmpMessage> e)
        {
            this._logger.LogInformation("-------------------client:connection is connected---------------------");
            this._logger.LogInformation(e.Context.Id);
        }

        private EndPoint ConvertIPV4EndPoint(IPEndPoint endpoint)
        {
            return new IPEndPoint(endpoint.Address.MapToIPv4(), endpoint.Port);
        }
        #endregion

        public async Task CloseTransportAsync(EndPoint endpoint)
        {
            this._logger.LogInformation("close transport {0}", endpoint);
            if (this.TRANSPORT_CACHE.TryRemove(endpoint, out var transport))
            {
                await transport.CloseAsync(CancellationToken.None).AnyContext();
                this._logger.LogInformation("close transport {0},completed", endpoint);
            }
        }

        public async Task<ITransport<AmpMessage>> CreateTransport(EndPoint endpoint)
        {
            if(this.TRANSPORT_CACHE.TryGetValue(endpoint,out var transport))
            {
                return transport;
            }

            await semaphoreSlim.WaitAsync();
            try
            {
                if (this.TRANSPORT_CACHE.TryGetValue(endpoint, out transport))
                {
                    return transport;
                }

                var context = await this._socket.ConnectAsync(endpoint);
                transport = new DefaultTransport(context);
                this.TRANSPORT_CACHE.AddOrUpdate(endpoint, transport, (k, old) => {
                    //Dispose
                    old.CloseAsync(CancellationToken.None).AnyContext();
                    return transport;
                });

                return transport;
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
        }

        public async Task CloseAllTransports(CancellationToken cancellationToken)
        {
            foreach(var transport in this.TRANSPORT_CACHE.Values)
            {
                await transport.CloseAsync(CancellationToken.None).AnyContext();
            }

            this.TRANSPORT_CACHE.Clear();
        }
    }
}
