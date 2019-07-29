using System.Threading.Tasks;
using Tomato.Rpc;

namespace MathService.Definition
{
    /// <summary>
    /// 测试服务
    /// </summary>
    [RpcService(101)]
    public interface IFooService
    {

        /// <summary>
        /// 测试方法
        /// </summary>
        /// <param name="req">请求</param>
        /// <returns>返回值</returns>
        [RpcMethod(1)]
        Task<RpcResult<FooRes>> FooAsync(FooReq req);
    }
}
