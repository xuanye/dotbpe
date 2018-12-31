using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Peach.Infrastructure;

namespace Peach.Hosting
{
    public class PeachHostedService : IHostedService 
    {
        private readonly IServerBootstrap _server;
        public PeachHostedService(IServerBootstrap server)
        {
            Preconditions.CheckNotNull(server, nameof(server));
            this._server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this._server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this._server.StopAsync(cancellationToken);
        }
       
    }
}