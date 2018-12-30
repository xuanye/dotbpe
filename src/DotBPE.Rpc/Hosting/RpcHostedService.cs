using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Peach;
using Peach.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Hosting
{
  
    public class RpcHostedService : IHostedService
    {
        private readonly IServerBootstrap _server;
        private readonly IServiceProvider _provider;
        private readonly ILoggerFactory _loggerFactory;
        public RpcHostedService(IServerBootstrap server,IServiceProvider provider)
        {
            Preconditions.CheckNotNull(server, nameof(server));
            this._server = server;
            this._provider = provider;
            this._loggerFactory = provider.GetService<ILoggerFactory>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SetEnvironment();
            return this._server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           
            return this._server.StopAsync(cancellationToken);
        }

        private void SetEnvironment()
        {
            Internal.Environment.SetServiceProvider(_provider);
            Internal.Environment.SetLoggerFactory(_loggerFactory);
        }

    }
}
