//using DotBPE.Rpc.Config;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// default implement for IRpcClient
    /// </summary>
    public class DefaultRpcClient : IRpcClient<AmpMessage>
    {
        //private readonly RpcClientOptions _clientOptions;
        private readonly IServiceRouter _serviceRouter;
        private readonly ITransportFactory<AmpMessage> _transportFactory;
        private readonly ILogger<DefaultRpcClient> _logger;
        private readonly IClientMessageHandler<AmpMessage> _handler;

        public DefaultRpcClient(
            //IOptions<RpcClientOptions> clientOptions,
            IServiceRouter serviceRouter,
            ITransportFactory<AmpMessage> transportFactory,
            IClientMessageHandler<AmpMessage> handler,
            ILogger<DefaultRpcClient> logger
            )
        {
            //_clientOptions = clientOptions.Value ?? new RpcClientOptions();
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
            var point = this._serviceRouter.FindRouterPoint(message.MethodIdentifier);

            if (point == null)
            {
                this._logger.LogError("route point not found !");
                ErrorResponse(message);
                return;
            }
            if(point.RoutePointType != RoutePointType.Remote){
                this._logger.LogError("route error");
                ErrorResponse(message);
                return;
            }
            var transport = await this._transportFactory.CreateTransport(point.RemoteAddress);
            await transport.SendAsync(message);
        }

        #endregion IRpcClient



        private void ErrorResponse(AmpMessage message)
        {
            var rsp = AmpMessage.CreateResponseMessage(message.ServiceId, message.MessageId);
            rsp.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
            this._handler.RaiseReceive(rsp);
        }
    }
}
