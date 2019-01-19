using System.Threading.Tasks;
using DotBPE.Rpc.Server.Impl;
using DotBPE.Rpc.Tests.Server;

namespace DotBPE.Rpc.Tests
{
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
}
