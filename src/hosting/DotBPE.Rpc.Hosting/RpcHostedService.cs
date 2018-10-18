using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
    /// <summary>
    /// 挂载RPC服务的宿主服务
    /// </summary>
    public class RpcHostedService : IHostedService
    {
        private readonly IServiceProvider _hostProvider;

        private readonly ILogger<RpcHostedService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private string _hostIP;
        private int _hostPort;

        private IServerBootstrap _sos;
        private IClientBootstrap _soc;

        public RpcHostedService(IServiceProvider hostProvider, IConfiguration config, ILogger<RpcHostedService> logger, ILoggerFactory loggerFactory)
        {
            this._hostProvider = hostProvider;
            this._logger = logger;
            this._loggerFactory = loggerFactory;

            ParseHostAddress(config);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return StartServerAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return StopServerAsync(cancellationToken);
        }

        private void ParseHostAddress(IConfiguration config)
        {
            string localAddress = config[HostDefaultKey.HOSTADDRESS_KEY];
            if (string.IsNullOrEmpty(localAddress))
            {
                localAddress = "0.0.0.0:6201";
            }
            string[] arr_Address = localAddress.Split(':');
            if (arr_Address.Length != 2)
            {
                throw new ArgumentException("server address error:" + localAddress);
            }
            this._hostIP = arr_Address[0];
            this._hostPort = int.Parse(arr_Address[1]);
        }

        /// <summary>
        /// 实现启动服务的部分
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task StartServerAsync(CancellationToken token)
        {
            BuildApplication();
            var endpoint = new IPEndPoint(IPAddress.Parse(this._hostIP), this._hostPort);
            await this._sos.StartAsync(endpoint, token);
            await this._soc.StartAsync();
        }

        /// <summary>
        /// 关闭服务器相关
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task StopServerAsync(CancellationToken token)
        {
            await this._sos.ShutdownAsync();
            await this._soc.StopAsync();
        }

        /// <summary>
        /// 创建一个RPC Application
        /// </summary>
        private void BuildApplication()
        {
            EnsureServer();
            //设置容器
            Rpc.Environment.SetServiceProvider(this._hostProvider);
            //设置日志工厂类
            Rpc.Environment.SetLoggerFactory(this._loggerFactory);
        }

        private void EnsureServer()
        {
            if (_sos == null)
            {
                _sos = _hostProvider.GetRequiredService<IServerBootstrap>();
            }

            if (_soc == null)
            {
                _soc = _hostProvider.GetRequiredService<IClientBootstrap>();
            }
        }
    }
}
