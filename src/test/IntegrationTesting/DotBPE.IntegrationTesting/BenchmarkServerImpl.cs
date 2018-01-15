using DotBPE.Rpc;
using System;
using System.Threading.Tasks;

namespace DotBPE.IntegrationTesting
{
    public class BenchmarkServerImpl:BenchmarkTestBase
    {
        public override Task<RpcResult<BenchmarkMessage>> EchoAsync(BenchmarkMessage request)
        {
            request.Field1 = "OK";
            request.Field2 = 100;

            var res = new RpcResult<BenchmarkMessage>();
            res.Data = request;
            return Task.FromResult(res);
        }
    }
}
