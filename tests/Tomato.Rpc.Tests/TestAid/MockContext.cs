using System.Net;
using System.Threading.Tasks;
using Tomato.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Peach;

namespace Tomato.Rpc.Tests
{
    public class MockContext : ISocketContext<AmpMessage>
    {
        public AmpMessage ResponseMessage { get; private set; }
        public string Id { get; }
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public IChannel Channel { get; }
        public bool Active { get; }

        public Task SendAsync(AmpMessage resMsg)
        {
            this.ResponseMessage = resMsg;
            return Task.CompletedTask;
        }
    }
}
