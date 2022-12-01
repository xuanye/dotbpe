// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    /// <summary>
    /// default implement for IRpcClient
    /// </summary>
    public class DefaultRpcClient : IRpcClient
    {
        private readonly IServiceRouter _serviceRouter;
        private readonly ITransportFactory _transportFactory;
        private readonly ILogger<DefaultRpcClient> _logger;
        private readonly IClientMessageHandler _handler;

        public DefaultRpcClient(
            IServiceRouter serviceRouter,
            ITransportFactory transportFactory,
            IClientMessageHandler handler,
            ILogger<DefaultRpcClient> logger
            )
        {
            _serviceRouter = serviceRouter;
            _transportFactory = transportFactory;
            _handler = handler;
            _logger = logger;
        }


        #region IRpcClient

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return _transportFactory.CloseAllTransports(cancellationToken);
        }

        public async Task SendAsync(AmpMessage message)
        {
            var point = await _serviceRouter.FindRouterPoint(message.RoutePath);

            if (point == null)
            {
                _logger.LogError("route point is not found !");
                RaiseErrorResponse(message);
                return;
            }
            if (point.RoutePointType != RoutePointType.Remote)
            {
                _logger.LogError("route error");
                RaiseErrorResponse(message);
                return;
            }
            var transport = await _transportFactory.CreateTransport(point.RemoteAddress);
            await transport.SendAsync(message);
        }

        #endregion IRpcClient


        private void RaiseErrorResponse(AmpMessage message)
        {
            var rsp = AmpMessage.CreateResponseMessage(message);
            rsp.Code = RpcStatusCodes.CODE_INTERNAL_ERROR;
            _handler.RaiseReceive(rsp);
        }
    }
}
