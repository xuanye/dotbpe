using Tomato.Rpc.Protocol;
using Tomato.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Tomato.Rpc.Tests.Server
{
    public class DefaultServiceActorLocatorTest
    {
        [Fact]
        public void LocateServiceActorTest()
        {
            var actor1 = new Mock<IServiceActor<AmpMessage>>();
            actor1.Setup(x => x.Id).Returns("actor1.0");

            var actor2 = new Mock<IServiceActor<AmpMessage>>();
            actor2.Setup(x => x.Id).Returns("actor2.0");

            IServiceCollection container = new ServiceCollection();

            container.AddLogging();
            container.BindServices(actors => { actors.Add(actor1.Object).Add(actor2.Object); });

            var provider = container.BuildServiceProvider();

            var locator = new DefaultServiceActorLocator(provider);

            var locatorActor1 = locator.LocateServiceActor("actor1.0");
            var locatorActor2 = locator.LocateServiceActor("actor2.0");

            Assert.Same(actor1.Object, locatorActor1);

            Assert.Same(actor2.Object, locatorActor2);
        }
    }
}
