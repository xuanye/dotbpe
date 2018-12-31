using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Server.Impl;

namespace MathService.Definition
{
    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult<FooRes>> FooAsync(FooReq req)
        {
            RpcResult<FooRes> result = new RpcResult<FooRes>();
            result.Data = new FooRes();
            result.Data.RetWord = req.FooWord;
            return Task.FromResult(result);
        }
    }
}
