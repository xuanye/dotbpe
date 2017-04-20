using System;
using System.Threading.Tasks;

namespace DotBPE.IntegrationTesting
{
    public class BenchmarkServerImpl:BenchmarkTestBase
    {
        private Action _action;

        public BenchmarkServerImpl(Action action)
        {
            this._action = action;
        }

        public override Task<BenchmarkMessage> EchoAsync(BenchmarkMessage request)
        {
            request.Field1 = "OK";
            request.Field2 = 100;
            return Task.FromResult(request);
        }
        public override Task<Void> QuitAsync(Void request){
            this._action?.Invoke();
            return Task.FromResult(new Void());
        }
    }
}
