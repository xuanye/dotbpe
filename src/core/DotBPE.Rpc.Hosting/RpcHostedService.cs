using DotBPE.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private bool _initialized;
        private bool _initializing; 
      
        private readonly ILogger<RpcHostedService>  _logger;
        private readonly ILoggerFactory _loggerFactory;  
        private string _hostIP;
        private int _hostPort;


        private IServerBootstrap _server;


        private Task _executingTask;
        private CancellationTokenSource _cts;

        public RpcHostedService(IServiceProvider hostProvider,  IConfiguration config,ILogger<RpcHostedService> logger,ILoggerFactory loggerFactory)
        {
            this._hostProvider = hostProvider;

            this._logger = logger;

            this._loggerFactory = loggerFactory;
            ParseHostAddress(config);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = StartServerAsync(_cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

       

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            //停止服务器
            await this.StopServerAsync(cancellationToken);

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
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
        private Task StartServerAsync(CancellationToken token)
        {
            this._logger.LogDebug("服务开始启动:{0}:{1}",this._hostIP,this._hostPort);
            Initialize();
            var endpoint = new IPEndPoint(IPAddress.Parse(this._hostIP), this._hostPort);
            return this._server.StartAsync(endpoint);
        }

        /// <summary>
        /// 关闭服务器相关
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task StopServerAsync(CancellationToken token)
        {
            this._logger.LogDebug("服务开始关闭<---");
            return this._server.ShutdownAsync();
        }



        private void Initialize()
        {
            if (!_initializing && !_initialized )
            {
                BuildApplication();
            }
        }

        /// <summary>
        /// 创建一个RPC Application
        /// </summary>
        private void BuildApplication()
        {
            if(_initializing || _initialized)
            {
                return;
            }

            _initializing = true;
          
            EnsureServer();

            //设置日志工厂类
            Rpc.Environment.SetLoggerFactory(this._loggerFactory);

            _initialized = true;
        }

       

        private void EnsureServer()
        {
            if (_server == null)
            {
                _server = _hostProvider.GetRequiredService<IServerBootstrap>();
            }
        }
    }
}
