using System.Threading.Tasks;
using DotBPE.Extra;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
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
            container.AddSingleton<IRequestAuditLoggerFactory, DefaultRequestAuditLoggerFactory>();
            Environment.SetServiceProvider(container.BuildServiceProvider());
        }


        [Fact]
        public void BaseServiceInitTest()
        {
            var fooService = new FooService();

            Assert.Equal("100.0", fooService.Id);
        }


        [Fact]
        public async Task BaseServiceProcessCallTest()
        {
            ISerializer serializer = new JsonSerializer();
            var fooService = new FooService();
            Assert.Equal("100.0", fooService.Id);


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

            var res = serializer.Deserialize<FooRes>(context1.ResponseMessage.Data);
            Assert.NotNull(res);

            Assert.Equal(req.FooWord, res.RetFooWord);
        }

        [Fact]
        public async Task BaseServiceReceiveTest()
        {
            var fooService = new FooService();

            Assert.Equal("100.0", fooService.Id);


            var context1 = new MockContext();
            var context2 = new MockContext();

            var reqMessage = AmpMessage.CreateRequestMessage(100, 1);
            reqMessage.Data = new byte[0];
            await fooService.ReceiveAsync(context1, reqMessage);

            Assert.NotNull(context1.ResponseMessage);
            Assert.Equal(0, context1.ResponseMessage.Code);

            reqMessage = AmpMessage.CreateRequestMessage(100, 2000);
            await fooService.ReceiveAsync(context2, reqMessage);

            Assert.NotNull(context2.ResponseMessage);
            Assert.Equal(RpcErrorCodes.CODE_SERVICE_NOT_FOUND, context2.ResponseMessage.Code);
        }
    }
}
