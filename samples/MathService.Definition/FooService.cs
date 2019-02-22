using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;

namespace MathService.Definition
{
    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult<FooRes>> FooAsync(FooReq req)
        {
            RpcResult<FooRes> result = new RpcResult<FooRes> {Data = new FooRes {RetWord = req.FooWord}};
            return Task.FromResult(result);
        }
    }
}
