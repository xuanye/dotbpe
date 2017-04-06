using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc
{
    public class DefaultServerHost : IServerHost
    {
        private readonly IServerBootstrap _bootstrap;
        private readonly ILogger _logger;

        public DefaultServerHost(IServerBootstrap bootstrap,ILogger logger)
        {
            this._bootstrap = bootstrap;
            this._logger = logger;
        }

        public void Dispose()
        {
            _bootstrap?.Dispose();
        }

        public Task StartAsync(EndPoint endpoint)
        {
            this._logger.LogDebug("服务正在{0}启动中...", endpoint.ToString());
            return this._bootstrap.StartAsync(endpoint);
        }
    }
}
