using System.Threading.Tasks;

namespace DotBPE.Rpc.Tests
{
    [RpcService(100, GroupName = "mock")]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult> Foo1Async(FooReq req);

        [RpcMethod(2)]
        Task<RpcResult<FooRes>> Foo2Async(FooReq req);
    }
}
