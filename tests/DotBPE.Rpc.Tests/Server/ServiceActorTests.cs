using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DotBPE.Rpc.Codec;
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
            container.AddSingleton<ISerializer, JsonSerializer>();
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
            reqMessage.Data= new byte[0];
            await fooService.ReceiveAsync(context1, reqMessage);

            Assert.NotNull(context1.ResponseMessage);
            Assert.Equal(0, context1.ResponseMessage.Code);

            reqMessage = AmpMessage.CreateRequestMessage(100, 2000);
            await fooService.ReceiveAsync(context2, reqMessage);

            Assert.NotNull(context2.ResponseMessage);
            Assert.Equal(RpcErrorCodes.CODE_SERVICE_NOT_FOUND, context2.ResponseMessage.Code);
        }


        [Fact]
        public async Task BaseServiceProcessCallTest()
        {
            ISerializer serializer = new JsonSerializer();
            var fooService = new FooService();
            Assert.Equal("100$0", fooService.Id);


            var req = new FooReq {FooWord = "hello dotbpe"};

            var context1 = new MockContext();

            var reqMessage = AmpMessage.CreateRequestMessage(100, 2);
            reqMessage.Version = 1;
            reqMessage.CodecType = CodecType.JSON;
            reqMessage.Sequence = 1;
            reqMessage.Data = serializer.Serialize(req);

            await fooService.ReceiveAsync(context1, reqMessage);

            Assert.NotNull(context1.ResponseMessage);
            Assert.Equal(0, context1.ResponseMessage.Code);

            Assert.NotNull(context1.ResponseMessage.Data);

            FooRes res = serializer.Deserialize<FooRes>(context1.ResponseMessage.Data);
            Assert.NotNull(res);

            Assert.Equal(req.FooWord,res.RetFooWord);
        }

    }


    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult> Foo1Async(FooReq req)
        {
            return Task.FromResult(new RpcResult());
        }

        public Task<RpcResult<FooRes>> Foo2Async(FooReq req)
        {
            return Task.FromResult(new RpcResult<FooRes>{ Data = new FooRes { RetFooWord = req.FooWord}});
        }
    }


    [RpcService(100, GroupName = "mock")]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult> Foo1Async(FooReq req);

        [RpcMethod(2)]
        Task<RpcResult<FooRes>> Foo2Async(FooReq req);
    }

    [DataContract]
    public class FooReq
    {
        [DataMember(Order = 1,Name = "foo_word")]
        public string FooWord { get; set; }
    }
    [DataContract]
    public class FooRes
    {
        [DataMember(Order = 1,Name = "ret_foo_word")]
        public string RetFooWord { get; set; }
    }

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
