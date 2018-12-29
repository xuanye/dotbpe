using System;
using Xunit;
using Castle.DynamicProxy;
using DotBPE.Rpc;

namespace DotBPE.Extra.Castle.Tests
{
    public class ClientInterceptorTest
    {
        [Fact]
        public void TestInterceptorInterface()
        {
            var proxy = new ProxyGenerator();
            var service = proxy.CreateInterfaceProxyWithoutTarget<IFooService>(new ClientInterceptor(null));
            var ret = service.Foo(1);

            Assert.Equal(1, ret);
        }
    }

    [RpcService(1000)]
    public interface IFooService
    {
        [RpcMethod(1)]
        int Foo(int a);
    }
}
