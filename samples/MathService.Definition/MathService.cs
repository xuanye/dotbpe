using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Server.Impl;
using Microsoft.Extensions.Logging;
using DotBPE.Rpc.Client;

namespace MathService.Definition
{
    public class MathService: BaseService<IMathService>,IMathService
    {
        private readonly ILogger<MathService> _logger;
        private readonly IClientProxy _clientProxy;
        public MathService(IClientProxy clientProxy,ILogger<MathService> logger)
        {
            _logger = logger;
            _clientProxy = clientProxy;
        }
        public async Task<RpcResult<SumRes>> SumAsync(SumReq req)
        {
            RpcResult<SumRes> result = new RpcResult<SumRes>() { Data = new SumRes() };
            result.Data.Total = req.A + req.B;

            var fooService = _clientProxy.Create<IFooService>();
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

    [RpcService(100)]
    public interface IMathService
    {
        [RpcMethod(1)]
        Task<RpcResult<SumRes>> SumAsync(SumReq req);
    }

    [RpcService(101)]
    public interface IFooService
    {
        [RpcMethod(1)]
        Task<RpcResult<FooRes>> FooAsync(FooReq req);
    }

}
