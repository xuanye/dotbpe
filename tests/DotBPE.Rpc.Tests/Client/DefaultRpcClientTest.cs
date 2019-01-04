using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultRpcClientTest
    {
        [Fact]
        public async Task SendAsyncWithoutErrorTest()
        {

            var router = new Mock<IServiceRouter>();
            router.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(
                new RouterPoint
                {
                    RoutePointType = RoutePointType.Remote

                });

            var transport = new Mock<ITransport<AmpMessage>>();
            transport.Setup(x => x.SendAsync(It.IsAny<AmpMessage>())).Returns(Task.CompletedTask);

            var factory = new Mock<ITransportFactory<AmpMessage>>();
            factory.Setup(x => x.CreateTransport(It.IsAny<EndPoint>())).Returns(
                Task.FromResult(transport.Object));

            var handler = new Mock<IClientMessageHandler<AmpMessage>>();
            var logger = NullLogger<DefaultRpcClient>.Instance;


            var client = new DefaultRpcClient(router.Object,factory.Object,handler.Object,logger);

            await client.SendAsync(AmpMessage.CreateRequestMessage(100, 1));
        }
    }
}
