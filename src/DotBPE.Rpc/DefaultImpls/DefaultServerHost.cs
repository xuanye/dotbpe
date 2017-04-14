using System;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultServerHost : IServerHost
    {
        private readonly IServerBootstrap _bootstrap;
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultServerHost>();

        private readonly RpcHostOption _option;
        public DefaultServerHost(IServerBootstrap bootstrap, RpcHostOption option)
        {
            this._bootstrap = bootstrap;

            this._option = option;
        }

        public Task ShutdownAsync()
        {
            return this._bootstrap.ShutdownAsync();
        }

        public Task StartAsync()
        {
            Logger.Debug($"服务正在{_option.HostIP}:{_option.HostPort}启动中...");
            var endpoint = new IPEndPoint(IPAddress.Parse(_option.HostIP), _option.HostPort);
            return this._bootstrap.StartAsync(endpoint);
        }
    }
}
