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

        [Fact]
        public void GetRegisteredServiceActor_Should_BeOk()
        {
            var service = new ServiceCollection();

            service.AddSingleton<IServiceActor<IFooService>, FooService>();

            service.AddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();

            service.AddLogging();

            var provider = service.BuildServiceProvider();

            var locator = provider.GetRequiredService<IServiceActorLocator>();

            var actor1 = locator.LocateServiceActor("100.0");
            var actor2 = locator.LocateServiceActor("100.1");
            var actor3 = locator.LocateServiceActor("100.2");


            Assert.NotNull(actor1);
            Assert.NotNull(actor2);
            Assert.NotNull(actor3);

            Assert.Same(actor1, actor2);
            Assert.Same(actor1, actor3);



        }

        [Fact]
        public void GetUnRegisteredServiceActor_Should_ReturnNotFoundServiceActor()
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


        [Fact]
        public void GetHeartBeatServiceActor_Should_BeOk()
        {
            var service = new ServiceCollection();

            service.AddSingleton<IServiceActor<IFooService>, FooService>();

            service.AddSingleton<IServiceActorLocator, DefaultServiceActorLocator>();

            service.AddLogging();

            var provider = service.BuildServiceProvider();

            var locator = provider.GetRequiredService<IServiceActorLocator>();

            var actor1 = locator.LocateServiceActor("0.0");

            Assert.NotNull(actor1);

            Assert.Same(HeartBeatActor.Instance, actor1);
        }
    }
}
