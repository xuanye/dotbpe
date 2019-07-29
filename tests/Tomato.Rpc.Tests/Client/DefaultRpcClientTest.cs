using System.Net;
using System.Threading.Tasks;
using Tomato.Rpc.Client;
using Tomato.Rpc.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Tomato.Rpc.Tests.Client
{
    public class DefaultRpcClientTest
    {
        [Fact]
        public async Task SendAsyncWithoutErrorTest()
        {
            var router = new Mock<IServiceRouter>();
            router.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(
                Task.FromResult<IRouterPoint>(new RouterPoint
                {
                    RoutePointType = RoutePointType.Remote
                }));

            var transport = new Mock<ITransport<AmpMessage>>();
            transport.Setup(x => x.SendAsync(It.IsAny<AmpMessage>())).Returns(Task.CompletedTask);

            var factory = new Mock<ITransportFactory<AmpMessage>>();
            factory.Setup(x => x.CreateTransport(It.IsAny<EndPoint>())).Returns(
                Task.FromResult(transport.Object));

            var handler = new Mock<IClientMessageHandler<AmpMessage>>();
            var logger = NullLogger<DefaultRpcClient>.Instance;


            var client = new DefaultRpcClient(router.Object, factory.Object, handler.Object, logger);

            await client.SendAsync(AmpMessage.CreateRequestMessage(100, 1));
        }
    }
}
