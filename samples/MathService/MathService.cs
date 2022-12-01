// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using MathService.Definition;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MathService
{
    public class MathService : BaseService<IMathService>, IMathService
    {
        private readonly ILogger<MathService> _logger;

        public MathService(ILogger<MathService> logger)
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
