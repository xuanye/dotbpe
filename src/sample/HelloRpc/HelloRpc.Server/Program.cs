
using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using HelloRpc.Common;
using Microsoft.Extensions.Configuration;

namespace HelloRpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            var config = new ConfigurationBuilder()
                .AddJsonFile("dotbpe.config.json", optional: true)
                .Build();

            var host = new RpcHostBuilder()
                .UseConfiguration(config) //使用配置文件
                .AddRpcCore<AmpMessage>() //添加核心依赖
                .UseNettyServer<AmpMessage>()  //使用使用Netty默认实现
                .UseAmp() //使用Amp协议中的默认实现
                .UseAmpClient() //还要调用外部服务
                .AddServiceActors(actors =>
                {
                    actors.Add<GreeterImpl>()
                        .Add<MathImpl>();
                }) //注册服务
                .Build();

            host.StartAsync().Wait();
            // 服务预热，首先建立好 需要的链接
            host.Preheating().Wait();

            Console.WriteLine("按任意键退出服务，任意键不包括任何电源键/关机键");
            Console.ReadKey();

            host.ShutdownAsync().Wait();

        }
    }

    public class GreeterImpl : GreeterBase
    {
        static readonly DotBPE.Rpc.Logging.ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<GreeterImpl>();
        public override async Task<HelloResponse> HelloPlusAsync(HelloRequest request)
        {

            var addReq = new AddRequest();
            addReq.Left = 1;
            addReq.Right = 2;

            MathClient math  = ClientProxy.GetClient<MathClient>();
            var addRep = await math.AddAsnyc(addReq,3000);

            var reply = new HelloResponse() { Message = "Hello " + request.Name +" plus:"+addRep.Total};
            return reply;
        }
    }

    public class MathImpl:MathBase{
        public override Task<AddResponse> AddAsync(AddRequest request){
            var response = new AddResponse();
            response.Total = request.Left +request.Right;
            return Task.FromResult(response);
        }
    }

}