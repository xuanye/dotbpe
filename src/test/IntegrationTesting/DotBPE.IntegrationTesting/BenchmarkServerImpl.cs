using System;
using System.Threading.Tasks;

namespace DotBPE.IntegrationTesting
{
    public class BenchmarkServerImpl:BenchmarkTestBase
    {
        public override Task<BenchmarkMessage> EchoAsync(BenchmarkMessage request)
        {
            request.Field1 = "OK";
            request.Field2 = 100;
            return Task.FromResult(request);
        }
    }
}
