using DotBPE.Rpc;
using System.Threading.Tasks;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;

namespace MathService.Definition
{
    public class MathService: BaseService<IMathService>,IMathService
    {
        public Task<RpcResult<SumRes>> SumAsync(SumReq req)
        {
            var result = new RpcResult<SumRes> { Data = new SumRes() };
            result.Data.Total = req.A + req.B;

            Logger.LogInformation("A+B=C {A}+{B}={C}",req.A,req.B,result.Data.Total);

            return Task.FromResult(result);
        }
    }

}
