using CommandLine;
using System;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;

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

            var host = new RpcHostBuilder()
                .UseServer(ip,port)
                .UseStartup<QpsServerStartup>()
                .Build();
            await host.StartAsync();
            Console.WriteLine("Running qps worker server on " + string.Format("{0}:{1}", ip, port));
            Console.ReadKey();
            await host.ShutdownAsync();
            Console.WriteLine("server is Shutdown");
        }
    }


    public class QpsServerStartup : IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDotBPE(); // 使用AMP协议

            services.AddServiceActors<AmpMessage>( (actors) =>
            {
                actors.Add<BenchmarkServerImpl>()
                      .Add<CallContextTestImpl>();
            });
            // 上下文获取器
            services.AddSingleton<IContextAccessor<AmpMessage>, DefaultContextAccessor<AmpMessage>>();

            return services.BuildServiceProvider();
        }

        public void Configure(IAppBuilder app, IHostingEnvironment env)
        {

        }


    }
}
