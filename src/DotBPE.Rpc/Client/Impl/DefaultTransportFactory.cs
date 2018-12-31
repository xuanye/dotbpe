using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Protocol;
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
        ConcurrentDictionary<EndPoint, ITransport<AmpMessage>> TRANSPORT_CACHE = new ConcurrentDictionary<EndPoint, ITransport<AmpMessage>>();

        private readonly ISocketClient<AmpMessage> _socket;
        private readonly IClientMessageHandler<AmpMessage> _handler;

        public DefaultTransportFactory(ISocketClient<AmpMessage> socket, IClientMessageHandler<AmpMessage> handler)
        {
            this._socket = socket;
            this._handler = handler;
            this._socket.OnIdleState += _socket_OnIdleState;
            this._socket.OnReceived += _socket_OnReceived;
            this._socket.OnConnected += _socket_OnConnected;
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
            e.Context.SendAsync(AmpMessage.HEARBEAT);
        }

        /// <summary>
        /// on receive message ,raise message handler event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _socket_OnReceived(object sender, Peach.EventArgs.MessageReceivedEventArgs<AmpMessage> e)
        {
            this._handler.RaiseReceive(e.Message);
        }



        private void _socket_OnError(object sender, Peach.EventArgs.ErrorEventArgs<AmpMessage> e)
        {
          
        }

        private void _socket_OnDisconnected(object sender, Peach.EventArgs.DisconnectedEventArgs<AmpMessage> e)
        {
           
        }

        private void _socket_OnConnected(object sender, Peach.EventArgs.ConnectedEventArgs<AmpMessage> e)
        {
          
        }

        #endregion

        public async Task CloseTransportAsync(EndPoint endpoint)
        {
            if (this.TRANSPORT_CACHE.TryRemove(endpoint, out var transport))
            {
                await transport.CloseAsync(CancellationToken.None).AnyContext();              
            }
        }

        public async Task<ITransport<AmpMessage>> CreateTransport(EndPoint endpoint)
        {
            if(this.TRANSPORT_CACHE.TryGetValue(endpoint,out var transport))
            {
                return transport;
            }
            var context = await this._socket.ConnectAsync(endpoint);
            transport = new DefaultTransport(endpoint, context);
            this.TRANSPORT_CACHE.AddOrUpdate(endpoint, transport, (k, old) => {
                //Dispose
                old.CloseAsync(CancellationToken.None).AnyContext();
                return transport;
            } );

            return transport;
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
