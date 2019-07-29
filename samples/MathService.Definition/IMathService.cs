using System.Threading.Tasks;
using Tomato.Gateway;
using Tomato.Rpc;

namespace MathService.Definition
{
    /// <summary>
    /// 数学服务
    /// </summary>
    [RpcService(100)]
    public interface IMathService
    {
        /// <summary>
        /// 加法服务
        /// </summary>
        /// <param name="req">请求参数req</param>
        /// <returns>返回值Res</returns>
        [RpcMethod(1),Router("/api/math/sum")]
        Task<RpcResult<SumRes>> SumAsync(SumReq req);
    }
}
