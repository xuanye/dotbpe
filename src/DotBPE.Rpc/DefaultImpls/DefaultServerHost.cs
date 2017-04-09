using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultServerHost : IServerHost
    {
        private readonly IServerBootstrap _bootstrap;
        private readonly ILogger<DefaultServerHost> _logger;

        private readonly RpcHostOption _option;
        public DefaultServerHost(IServerBootstrap bootstrap,ILogger<DefaultServerHost> logger, RpcHostOption option)
        {
            this._bootstrap = bootstrap;
            this._logger = logger;
            this._option = option;
        }

        public void Dispose()
        {
            _bootstrap?.Dispose();
        }

        public Task StartAsync()
        {
            this._logger.LogDebug("服务正在{0}启动中...");
            Console.WriteLine($"DotRpc is starting at {_option.HostIP}:{_option.HostPort}");
            var endpoint = new IPEndPoint(IPAddress.Parse(_option.HostIP), _option.HostPort);
            return this._bootstrap.StartAsync(endpoint);
        }
    }
}
