using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.Logging;

namespace MathService.Definition
{
    public class ExtraCallFooMathService: BaseService<IMathService>,IMathService
    {
        private readonly ILogger<MathService> _logger;
        private readonly IClientProxy _clientProxy;
        public ExtraCallFooMathService(IClientProxy clientProxy,ILogger<MathService> logger)
        {
            this._logger = logger;
            this._clientProxy = clientProxy;
        }
        public async Task<RpcResult<SumRes>> SumAsync(SumReq req)
        {
            RpcResult<SumRes> result = new RpcResult<SumRes> { Data = new SumRes() };
            result.Data.Total = req.A + req.B;

            var fooService = this._clientProxy.Create<IFooService>();
            var fooResult = await fooService.FooAsync(new FooReq() { FooWord="Foo" });

            if(fooResult.Code != 0)
            {
                result.Code = fooResult.Code;
                return result;
            }
            result.Data.FooWord = fooResult.Data?.RetWord;

            this._logger.LogInformation("A+B=C {A}+{B}={C}",req.A,req.B,result.Data.Total);
            return result;
        }
    }

}
