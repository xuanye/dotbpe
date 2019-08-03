using System;
using System.Threading.Tasks;
using Tomato.Rpc;
using Tomato.Rpc.Server;

namespace MathService.Definition
{
    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult<FooRes>> FooAsync(FooReq req)
        {
            RpcResult<FooRes> result = new RpcResult<FooRes> {Data = new FooRes {RetWord = req.FooWord}};
            //throw  new Exception("测试异常");
            return Task.FromResult(result);
        }
    }
}
