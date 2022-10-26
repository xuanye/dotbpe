// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using Moq.AutoMock;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultTransportFactoryTests
    {
        [Theory]
        [InlineData("192.168.1.2", 4000)]
        [InlineData("127.0.0.1", 4001)]
        public async Task CreateTransport_ShouldBe_Ok(string address, int port)
        {
            //arrange
            var autoMocker = new AutoMocker();

            var factory = autoMocker.CreateInstance<DefaultTransportFactory>();

            var endpoint1 = new IPEndPoint(IPAddress.Parse(address), port);
            var endpoint2 = new IPEndPoint(IPAddress.Parse(address), port);
            var endpoint3 = new IPEndPoint(IPAddress.Parse(address), port + 1);

            //act

            var transport1 = await factory.CreateTransport(endpoint1);
            var transport2 = await factory.CreateTransport(endpoint2);
            var transport3 = await factory.CreateTransport(endpoint3);


            //assert
            Assert.NotNull(transport1);
            Assert.NotNull(transport2);
            Assert.NotNull(transport3);

            Assert.Equal(transport1, transport2);
            Assert.NotSame(transport1, transport3);

        }

        [Theory]
        [InlineData("192.168.1.2", 4000)]
        [InlineData("127.0.0.1", 4001)]
        public async Task CreateTransport_Concurrently_ShouldBe_Ok(string address, int port)
        {
            //arrange
            var autoMocker = new AutoMocker();

            var factory = autoMocker.CreateInstance<DefaultTransportFactory>();

            var endpoint1 = new IPEndPoint(IPAddress.Parse(address), port);
            var endpoint2 = new IPEndPoint(IPAddress.Parse(address), port);
            var endpoint3 = new IPEndPoint(IPAddress.Parse(address), port + 1);
            //act

            var task1 = factory.CreateTransport(endpoint1);
            var task2 = factory.CreateTransport(endpoint2);
            var task3 = factory.CreateTransport(endpoint3);

            await Task.WhenAll(task1, task2, task3);

            var transport1 = task1.Result;
            var transport2 = task2.Result;
            var transport3 = task3.Result;


            //assert
            Assert.NotNull(transport1);
            Assert.NotNull(transport2);
            Assert.NotNull(transport3);

            Assert.Equal(transport1, transport2);
            Assert.NotSame(transport1, transport3);

        }
    }
}
