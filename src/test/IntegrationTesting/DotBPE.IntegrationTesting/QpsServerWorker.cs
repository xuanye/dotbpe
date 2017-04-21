using CommandLine;
using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;

namespace DotBPE.IntegrationTesting
{
    public class ServerOptions{
       [Option("port", Default = 0)]
       public int Port{get;set;}
    }
    public class QpsServerWorker
    {
        private readonly ServerOptions _option;

        public QpsServerWorker(ServerOptions option)
        {
            this._option = option;
        }

        public static void Run(string[] args){
            var parserResult = Parser.Default.ParseArguments<ServerOptions>(args)
            .WithNotParsed(x=>System.Environment.Exit(1))
            .WithParsed( option=>{
               var work = new QpsServerWorker(option);
               work.RunAsync().Wait();
            });
        }

        private async Task RunAsync()
        {
            string ip = "0.0.0.0";
            int port = this._option.Port;

            var tcs = new TaskCompletionSource<object>();
            var workerServiceImpl = new BenchmarkServerImpl(() => { Task.Run(() => tcs.SetResult(null)); });

             var host = new RpcHostBuilder()
                .AddRpcCore<AmpMessage>() //添加核心依赖
                .UseNettyServer<AmpMessage>()  //使用使用Netty默认实现
                .UseAmp() //使用Amp协议中的默认实现
                .UseServer(ip,port)
                .AddServiceActors(actors =>
                {
                    actors.Add(workerServiceImpl);
                }) //注册服务
                .Build();
            await host.StartAsync();
            Console.WriteLine("Running qps worker server on " + string.Format("{0}:{1}", ip, port));

            await tcs.Task;
            await host.ShutdownAsync();
            Console.WriteLine("server is Shutdown");
        }
    }
}
