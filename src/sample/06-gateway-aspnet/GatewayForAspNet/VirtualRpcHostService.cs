using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayForAspNet
{
    /// <summary>
    /// 新版的IHostService 还没发布，所以暂时用这个顶替一下，兼容到asp.net core 2.0中，2.1发布后可移除
    /// </summary>
    public class VirtualRpcHostService : RpcHostedService, IHostedService
    {
        public VirtualRpcHostService(IServiceProvider hostProvider,
            IConfiguration config,
            ILogger<RpcHostedService> logger,
            ILoggerFactory loggerFactory) : base(hostProvider, config, logger, loggerFactory)
        {

        }
    }
}
