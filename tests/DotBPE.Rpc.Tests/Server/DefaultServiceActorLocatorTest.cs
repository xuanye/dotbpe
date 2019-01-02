using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class DefaultServiceActorLocatorTest
    {
        [Fact]
        public void LocateServiceActorTest()
        {
            var actor1 = new Mock<IServiceActor<AmpMessage>>();
            actor1.Setup(x => x.Id).Returns("actor1");

            var actor2 = new Mock<IServiceActor<AmpMessage>>();
            actor2.Setup(x => x.Id).Returns("actor2");

           IServiceCollection container = new ServiceCollection();

           container.AddLogging();
           container.BindServices(actors => { actors.Add(actor1.Object);actors.Add(actor2.Object); });

           var provider = container.BuildServiceProvider();

           DefaultServiceActorLocator locator = new DefaultServiceActorLocator(provider);

           var locatorActor1= locator.LocateServiceActor("actor1");
           var locatorActor2= locator.LocateServiceActor("actor2");

           Assert.Same(actor1.Object,locatorActor1);

           Assert.Same(actor2.Object,locatorActor2);

        }
    }
}
