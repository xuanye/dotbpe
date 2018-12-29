using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultRpcClient : IRpcClient<AmpMessage>
    {
        private readonly RpcClientOptions _clientOptions;
        private readonly IServiceActorLocator<AmpMessage> _actorLocator;
        private readonly IServiceRouter<AmpMessage> _serviceRouter;
        private readonly ITransportFactory<AmpMessage> _transportFactory;
        private readonly ILogger<DefaultRpcClient> _logger;
        private readonly IClientMessageHandler<AmpMessage> _handler;

        public DefaultRpcClient(
            IOptions<RpcClientOptions> clientOptions,
            IServiceRouter<AmpMessage> serviceRouter,
            IServiceActorLocator<AmpMessage> actorLocator,
            ITransportFactory<AmpMessage> transportFactory,
            IClientMessageHandler<AmpMessage> handler,
            ILogger<DefaultRpcClient> logger
            )
        {
            this._clientOptions = clientOptions.Value ?? new RpcClientOptions();
            this._actorLocator = actorLocator;
            this._serviceRouter = serviceRouter;
            this._transportFactory = transportFactory;
            this._handler = handler;
            this._logger = logger;
        }

   
        #region IRpcClient

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return this._transportFactory.CloseAllTransports(cancellationToken);
        }

        public async Task SendAsync(AmpMessage message)
        {
            var point = await _serviceRouter.FindRouterPoint(message);

            if (point == null)
            {
                this._logger.LogError("route point not found !");
                ErrorResponse(message);
                return;
            }

            if (point.RoutePointType == RoutePointType.Remote)
            {
                var transport = await this._transportFactory.CreateTransport(point.RemoteAddress);
                await transport.SendAsync(message);
            }
            else if (point.RoutePointType == RoutePointType.Local)
            {
                var serviceActor = this._actorLocator.LocateServiceActor(message);
                if (serviceActor != null)
                {
                    await serviceActor.ReceiveAsync(new InprocContext(this._handler), message);
                }
                else
                {
                    _logger.LogError("service not found ,ServiceIdentifier = {ServiceIdentifier}", message.ServiceIdentifier);
                    NotFoundResponse(message);
                }
                return;
            }
            _logger.LogError("error occ, smart route point is not supported ,ServiceIdentifier = {ServiceIdentifier}", message.ServiceIdentifier);
            ErrorResponse(message);
        }

        #endregion IRpcClient

        private void NotFoundResponse(AmpMessage message)
        {
            var rsp = AmpMessage.CreateResponseMessage(message.ServiceId, message.MessageId);
            rsp.Code = RpcErrorCodes.CODE_SERVICE_NOT_FOUND;
            this._handler.RaiseReceive(rsp);
        }

        private void ErrorResponse(AmpMessage message)
        {
            var rsp = AmpMessage.CreateResponseMessage(message.ServiceId, message.MessageId);
            rsp.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            this._handler.RaiseReceive(rsp);
        }
    }
}
