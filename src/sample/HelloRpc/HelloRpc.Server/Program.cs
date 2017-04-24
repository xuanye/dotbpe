
using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using HelloRpc.Common;
using Microsoft.Extensions.Configuration;
using DotBPE.Plugin.Logging;

namespace HelloRpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            NLoggerWrapper.InitConfig();
            DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(Program)));

            var host = new RpcHostBuilder()
                .AddRpcCore<AmpMessage>() //添加核心依赖
                .UseNettyServer<AmpMessage>()  //使用使用Netty默认实现
                .UseAmp() //使用Amp协议中的默认实现
                .AddServiceActors(actors =>
                {
                    actors.Add<GreeterImpl>();
                }) //注册服务
                .Build();

            host.StartAsync().Wait();

            DotBPE.Rpc.Environment.Logger.Debug("Press any key to quit!");
            Console.ReadKey();

            host.ShutdownAsync().Wait();

        }
    }

    public class GreeterImpl : GreeterBase
    {
        public override Task<HelloResponse> HelloAsync(HelloRequest request)
        {
            return Task.FromResult(new HelloResponse() { Message = "Hello " + request.Name });
        }
    }

}
