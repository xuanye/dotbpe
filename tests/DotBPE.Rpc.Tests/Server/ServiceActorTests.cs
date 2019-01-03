using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server.Impl;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Peach;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class ServiceActorTests
    {

        public ServiceActorTests()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddLogging();

            var serializer = new Mock<ISerializer>();

            container.AddSingleton(typeof(ISerializer), serializer.Object);

            Internal.Environment.SetServiceProvider( container.BuildServiceProvider());
        }


        [Fact]
        public void BaseServiceInitTest()
        {
            var fooService = new FooService();

            Assert.Equal("100$0", fooService.Id);
        }

        [Fact]
        public async Task BaseServiceReceiveTest()
        {
            var fooService = new FooService();

            Assert.Equal("100$0", fooService.Id);


            var context1 = new MockContext();
            var context2 = new MockContext();

            var reqMessage = AmpMessage.CreateRequestMessage(100, 1);
            await fooService.ReceiveAsync(context1, reqMessage);

            Assert.Equal(0, context1.ReceiveCode);

            reqMessage = AmpMessage.CreateRequestMessage(100, 2);
            await fooService.ReceiveAsync(context2, reqMessage);

            Assert.Equal(RpcErrorCodes.CODE_SERVICE_NOT_FOUND, context2.ReceiveCode);
        }

    }


    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult> Foo1Async(FooReq req)
        {
            return Task.FromResult(new RpcResult());
        }
    }


    [RpcService(100, GroupName = "mock")]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult> Foo1Async(FooReq req);
    }

    public class FooReq
    {
        private string FooWord { get; set; }
    }


    public class MockContext : ISocketContext<AmpMessage>
    {
        public string Id { get; }
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public IChannel Channel { get; }
        public bool Active { get; }


        public int ReceiveCode { get; private set; }

        public Task SendAsync(AmpMessage message)
        {
            this.ReceiveCode = message.Code;
            return Task.CompletedTask;
        }
    }
}
