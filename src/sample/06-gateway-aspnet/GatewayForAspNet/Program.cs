
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Plugin.AspNetGateway;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GatewayForAspNet
{
    class Program
    {
        static void Main(string[] args)
        {

            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            // 启动HTTP服务
            var urls = "http://127.0.0.1:6200";
            var webHost = new WebHostBuilder()
             .UseKestrel()
             .UseUrls(urls)
             .UseStartup<Startup>()
             .Build();

            webHost.StartAsync().Wait();

            Console.WriteLine("running http server listen {0}", urls);

            // RPC Server
            string ip = "127.0.0.1";
            int port = 6201;
            
            var rpcHost = new RpcHostBuilder()
                .UseServer(ip, 6201)
                .UseStartup<DotBPEStartup>()
                .Build();

            rpcHost.StartAsync().Wait();
            Console.WriteLine("running server on {0}:{1}", ip, port);
            Console.WriteLine("press any key to shutdown");
            Console.ReadKey();

            webHost.StopAsync().Wait();
            Console.WriteLine("http server is shutdown");

            rpcHost.ShutdownAsync().Wait();
            Console.WriteLine("dotbpe server is shutdown");

        }

    }
    
}
