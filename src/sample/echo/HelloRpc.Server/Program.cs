
using System;
using System.Net;
using System.Text;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Logging;
using HelloRpc.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloRpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var config = new ConfigurationBuilder()
                .AddJsonFile("dotbpe.config.json", optional: true)
                .Build();

            var host = new RpcHostBuilder()
                .UseConfiguration(config) //使用配置文件
                .AddRpcCore<AmpMessage>() //添加核心依赖
                .UseNettyServer<AmpMessage>()  //使用使用Netty默认实现
                .UseAmp() //使用Amp协议中的默认实现
                .UseAmpClient() //还要调用外部服务
                .AddServiceActor(new GreeterImpl())  //注册服务
                .Build();

            host.StartAsync().Wait();

            Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            Console.ReadLine();
            host.ShutdownAsync().Wait();

        }
    }

    public class GreeterImpl : GreeterBase
    {
        public override async Task<HelloResponse> HelloPlusAsync(HelloRequest request)
        {
            var addReq = new addRequest();
            addReq.Left = 1;
            addReq.Right = 2;

            MathClient math  = ClientProxy.GetClient<MathClient>();

            var  addRep = await math.AddAsnyc(addReq);
            var reply = new HelloResponse() { Message = "Hello " + request.Name +" 1+2 =" +addRep.Total };
            return reply;
        }
    }

}