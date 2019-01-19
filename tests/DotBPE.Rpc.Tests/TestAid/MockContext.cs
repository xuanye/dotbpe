using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Peach;

namespace DotBPE.Rpc.Tests
{
    public class MockContext : ISocketContext<AmpMessage>
    {
        public string Id { get; }
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public IChannel Channel { get; }
        public bool Active { get; }


        public AmpMessage ResponseMessage { get; private set; }

        public Task SendAsync(AmpMessage resMsg)
        {
            ResponseMessage = resMsg;
            return Task.CompletedTask;
        }
    }
}
