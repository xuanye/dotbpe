using System.Threading.Tasks;
using DotBPE.Rpc;

namespace MathService.Definition
{
    [RpcService(100)]
    public interface IMathService
    {
        [RpcMethod(1)]
        Task<RpcResult<SumRes>> SumAsync(SumReq req);
    }
}
