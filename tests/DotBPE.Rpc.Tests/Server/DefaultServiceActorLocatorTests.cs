// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Server;
using DotBPE.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class DefaultServiceActorLocatorTests
    {


        [Theory]
        [InlineData("100.0", "100.1")]
        [InlineData("100.0", "100.2")]
        public void GetRegisteredServiceActor_Should_BeOk(string serviceId, string methodId)
        {
            //arrange
            var service = new ServiceCollection();
            service.AddSingleton<IServiceActor<IFooService>, FooService>();
            service.AddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();
            service.AddLogging();
            var provider = service.BuildServiceProvider();
            var locator = provider.GetRequiredService<IServiceActorLocator>();


            //act
            var serviceActor = locator.LocateServiceActor(serviceId);
            var methodActor = locator.LocateServiceActor(methodId);


            //assert
            Assert.NotNull(serviceActor);
            Assert.NotNull(methodActor);
            Assert.Same(serviceActor, methodActor);



        }

        [Fact]
        public void LocateServiceActor_ReturnNotFoundServiceActor_WithoutRegistration()
        {
            var service = new ServiceCollection();

            service.AddSingleton<IServiceActor<IFooService>, FooService>();

            service.AddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();

            service.AddLogging();

            var provider = service.BuildServiceProvider();

            var locator = provider.GetRequiredService<IServiceActorLocator>();

            var actor1 = locator.LocateServiceActor("100.0");

            var actor2 = locator.LocateServiceActor("101.0");

            Assert.NotNull(actor1);
            Assert.NotNull(actor2);
            Assert.Same(NotFoundServiceActor.Instance, actor2);
        }

    }
}
