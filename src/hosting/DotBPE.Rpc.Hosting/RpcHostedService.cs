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
     
        private readonly ILogger<RpcHostedService>  _logger;
        private readonly ILoggerFactory _loggerFactory;
        private string _hostIP;
        private int _hostPort;


        private IServerBootstrap _server;
        

        public RpcHostedService(IServiceProvider hostProvider,  IConfiguration config,ILogger<RpcHostedService> logger,ILoggerFactory loggerFactory)
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


        public  Task StopAsync(CancellationToken cancellationToken)
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
        private Task StartServerAsync(CancellationToken token)
        {
            BuildApplication();
            var endpoint = new IPEndPoint(IPAddress.Parse(this._hostIP), this._hostPort);           
            return this._server.StartAsync(endpoint,token);
        }

        /// <summary>
        /// 关闭服务器相关
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task StopServerAsync(CancellationToken token)
        {           
            return this._server.ShutdownAsync();           
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
            if (_server == null)
            {
                _server = _hostProvider.GetRequiredService<IServerBootstrap>();
            }
        }
    }
}
