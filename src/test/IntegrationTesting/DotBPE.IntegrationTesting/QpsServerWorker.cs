using CommandLine;
using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.Hosting;
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

            var host = new HostBuilder()
                .UseServer(ip,port)
                .ConfigureServices( services =>
                {
                    services.AddDotBPE(); // 使用AMP协议
                    // 添加业务服务的代码
                    services.AddServiceActors<AmpMessage>((actors) =>
                    {
                        actors.Add<BenchmarkServerImpl>()
                              .Add<CallContextTestImpl>();
                    });

                    // 上下文获取器
                    services.AddSingleton<IContextAccessor<AmpMessage>, DefaultContextAccessor<AmpMessage>>();
                })
                .Build();

            using (host)
            {
                Console.WriteLine("Running qps worker server on " + string.Format("{0}:{1}", ip, port));

                await host.StartAsync();

                Console.WriteLine("Started! Press <enter> to stop.");

                Console.ReadLine();

                Console.WriteLine("Stopping!");

                await host.StopAsync();

                Console.WriteLine("server is Shutdown");
            }
          
        }
    }
    
}
