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
            _server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync(cancellationToken);
        }
       
    }
}