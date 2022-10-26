// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MathService.Definition
{
    public class ExtraCallFooMathService : BaseService<IMathService>, IMathService
    {
        private readonly ILogger<MathService> _logger;

        public ExtraCallFooMathService(ILogger<MathService> logger)
        {
            _logger = logger;

        }
        public Task<RpcResult<SumRes>> SumAsync(SumReq req)
        {
            var result = new RpcResult<SumRes> { Data = new SumRes() };
            result.Data.Total = req.A + req.B;

            _logger.LogInformation("A+B=C {A}+{B}={C}", req.A, req.B, result.Data.Total);
            return Task.FromResult(result);
        }
    }

}
