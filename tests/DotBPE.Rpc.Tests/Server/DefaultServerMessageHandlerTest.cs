using System.Threading.Tasks;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Peach;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class DefaultServerMessageHandlerTest
    {
        [Fact]
        public Task ReceiveAsyncWithNoExceptionTest()
        {
            //ISocketContext<AmpMessage> context, AmpMessage message

            var actor = new Mock<IServiceActor<AmpMessage>>();
            actor.Setup(
                    x =>
                        x.ReceiveAsync(It.IsAny<ISocketContext<AmpMessage>>(), It.IsAny<AmpMessage>()))
                .Returns(Task.CompletedTask);

            var actorLocator = new Mock<IServiceActorLocator<AmpMessage>>();
            actorLocator.Setup(x => x.LocateServiceActor(It.IsAny<string>())).Returns(actor.Object);


            var logger = NullLogger<DefaultServerMessageHandler>.Instance;

            var handler = new DefaultServerMessageHandler(actorLocator.Object, logger);

            return handler.ReceiveAsync(null, AmpMessage.CreateRequestMessage(1, 1));
        }
    }
}
