using System.Runtime.Serialization;
using System.Threading.Tasks;
using DotBPE.Gateway;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Server.Impl;

namespace GreeterHttpService
{
    [RpcService(101)]
    public interface IGreeterService:IRpcService
    {
        [RpcMethod(1),Router("/api/greeter/hello")]
        Task<RpcResult<SayHelloRes>> SayHelloAsync(SayHelloReq req);
    }


    public class GreeterService : BaseService<IGreeterService>, IGreeterService
    {
        public Task<RpcResult<SayHelloRes>> SayHelloAsync(SayHelloReq req)
        {
            var result = new RpcResult<SayHelloRes>
            {
                Data = new SayHelloRes {Greeting = $"Hello {req.Name},{req.Id}!"}
            };

            return Task.FromResult(result);
        }
    }


    [DataContract]
    public class SayHelloReq
    {
        [DataMember(Order = 10,Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 11,Name = "id")]
        public int Id { get; set; }

    }

    [DataContract]
    public class SayHelloRes
    {

         [DataMember(Order = 1,Name = "returnMessage")]
         public string ReturnMessage { get; set; }

         [DataMember(Order = 10,Name = "greeting")]
         public string Greeting { get; set; }
    }
}
