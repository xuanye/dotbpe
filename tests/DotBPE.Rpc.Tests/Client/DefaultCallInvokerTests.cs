using System.Threading.Tasks;
using DotBPE.Extra;
using DotBPE.Rpc.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultCallInvokerTests
    {
        [Fact]
        public async Task AsyncNotifyTest()
        {
            /*
             *   IClientMessageHandler<AmpMessage> handler,
            IRpcClient<AmpMessage> rpcClient,
            ISerializer serializer,
            ILogger<DefaultCallInvoker> logger
             */

            var handler = new DefaultClientMessageHandler();
            var client = new MockRpcClient(handler);

            var serializer = new JsonSerializer();
            var logger = NullLogger<DefaultCallInvoker>.Instance;

            var invoker = new DefaultCallInvoker(handler, client, serializer, logger);

            var req = new FooReq {FooWord = "hello dotbpe"};

            var result = await invoker.AsyncNotify("FooService.Foo", "default", 100, 1, req);

            Assert.NotNull(result);
            Assert.Equal(0, result.Code);
            Assert.NotNull(client.ReceiveMessage);

            Assert.Equal(100, client.ReceiveMessage.ServiceId);
            Assert.Equal(1, client.ReceiveMessage.MessageId);

            Assert.Equal("FooService.Foo", client.ReceiveMessage.FriendlyServiceName);
        }

        [Fact]
        public async Task AsyncRequestTest()
        {
            /*
             *   IClientMessageHandler<AmpMessage> handler,
            IRpcClient<AmpMessage> rpcClient,
            ISerializer serializer,
            ILogger<DefaultCallInvoker> logger
             */
            var serializer = new JsonSerializer();
            var handler = new DefaultClientMessageHandler();

            var client = new MockRpcClient2(serializer, handler);
            var logger = NullLogger<DefaultCallInvoker>.Instance;

            var invoker = new DefaultCallInvoker(handler, client, serializer, logger);

            var req = new FooReq {FooWord = "hello dotbpe"};

            var result = await invoker.AsyncRequest<FooReq, FooRes>("FooService.Foo", "default", 100, 1, req);

            Assert.NotNull(result);
            Assert.Equal(0, result.Code);


            Assert.NotNull(result.Data);
            Assert.Equal(req.FooWord, result.Data.RetFooWord);
        }
    }
}
