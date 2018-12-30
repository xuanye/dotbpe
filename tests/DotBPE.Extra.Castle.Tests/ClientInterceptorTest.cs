using System;
using Xunit;
using Castle.DynamicProxy;
using DotBPE.Rpc;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using Moq;

namespace DotBPE.Extra.Castle.Tests
{
    public class ClientInterceptorTest
    {
        [Fact]
        public async Task TestInterceptorInterface()
        {
            var req = 1;
            var result1 = new RpcResult();
            var result2 = new RpcResult<int>();
            result2.Data = req;

            var invoke = new Mock<ICallInvoker>();
            invoke.Setup(x => x.AsyncCallWithOutResponse<int>
           (It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<int>()))
            .Returns(Task.FromResult(result1));

            invoke.Setup(x => x.AsyncCall<int,int>
           (It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<int>(),It.IsAny<int>()))
            .Returns(Task.FromResult(result2));

            var proxy = new ProxyGenerator();
            var service = proxy.CreateInterfaceProxyWithoutTarget<IFooService>(new ClientInterceptor(invoke.Object));
            var ret = await service.Foo(1);

            Assert.Equal(0, ret.Code);
            Assert.Equal(1, ret.Data);

            //CACHE TEST
            ret = await service.Foo(1);
            Assert.Equal(0, ret.Code);
            Assert.Equal(1, ret.Data);

            var ret2 = await service.FooNoRsponse(1);
            Assert.Equal(0, ret2.Code);          
        }
    }

    [RpcService(1000)]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult<int>> Foo(int a);


        [RpcMethod(2)]
        Task<RpcResult> FooNoRsponse(int a);
    }
}
