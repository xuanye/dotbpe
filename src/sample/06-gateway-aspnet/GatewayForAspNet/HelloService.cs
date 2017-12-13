using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GatewayForAspNet
{
    public class GreeterService : GreeterBase
    {
        public override Task<HelloRes> SayHelloAgainAsync(HelloReq request)
        {
            var res = new HelloRes()
            {
                GreetWord = $"Hello {request.Name} Again!"
            };
            return Task.FromResult(res);
        }

        public override Task<HelloRes> SayHelloAsync(HelloReq request)
        {
            var res = new HelloRes()
            {
                GreetWord = $"Hello {request.Name} !"
            };
            return Task.FromResult(res);
        }
    }
}
