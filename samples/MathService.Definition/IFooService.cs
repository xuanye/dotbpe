using System.Threading.Tasks;
using DotBPE.Rpc;

namespace MathService.Definition
{
    [RpcService(101)]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult<FooRes>> FooAsync(FooReq req);
    }
}
