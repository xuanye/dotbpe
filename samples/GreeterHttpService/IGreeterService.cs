using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DotBPE.Gateway;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;

namespace GreeterHttpService
{
    /// <summary>
    /// IGreeterService Greet服务
    /// </summary>
    [RpcService(101)]
    public interface IGreeterService:IRpcService
    {
        /// <summary>
        ///  Say Hello 方法
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [RpcMethod(1),Router("/api/greeter/hello")]
        Task<RpcResult<SayHelloRes>> SayHelloAsync(SayHelloReq req);
    }


    /// <inheritdoc />
    public class GreeterService : BaseService<IGreeterService>, IGreeterService
    {
        public Task<RpcResult<SayHelloRes>> SayHelloAsync(SayHelloReq req)
        {
            var result = new RpcResult<SayHelloRes>
            {
                Code = 0,
                Data = new SayHelloRes {Greeting = $"Hello {req.Name}" , ReturnMessage = ""}
            };

            //throw  new Exception("测试异常");
            return Task.FromResult(result);
        }
    }


    /// <summary>
    /// SayHello的请求消息
    /// </summary>
    [DataContract]
    public class SayHelloReq
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Order = 10,Name = "name")]
        public string Name { get; set; }
       

    }

    /// <summary>
    /// Say Hello响应消息
    /// </summary>
    [DataContract]
    public class SayHelloRes
    {

        /// <summary>
        /// 返回的消息
        /// </summary>
         [DataMember(Order = 1,Name = "returnMessage")]
         public string ReturnMessage { get; set; }


        /// <summary>
        /// Greeting words
        /// </summary>
         [DataMember(Order = 10,Name = "greeting")]
         public string Greeting { get; set; }

    }

    [DataContract]
    public class SampleData
    {
        [DataMember(Order = 1,Name = "list")]
        public List<SampleData> Children { get; } = new List<SampleData>();
    }
}
