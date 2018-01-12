using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Logging;
using MathCommon;
using Microsoft.Extensions.DependencyInjection;
using Google.Protobuf;


namespace MathServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            string ip = "127.0.0.1";
            int port = 6201;

            var host = new RpcHostBuilder()
                .UseServer(ip, 6201)
                .UseStartup<ServerStartup>()
                .Build();

            host.StartAsync().Wait();
            Console.WriteLine("running server on {0}:{1}" , ip, port);
            Console.WriteLine("press any key to shutdown");
            Console.ReadKey();

            host.ShutdownAsync().Wait();
            Console.WriteLine("server is shutdown");
        }
    }

    public class MathService : MathBase
    {
        public override Task<RpcResult<AddRes>> AddAsync(AddReq req){
            var res = new AddRes();
            res.C  = req.A + req.B ;
            return Task.FromResult(new RpcResult<AddRes>() { Data = res });
        }
    }

    public class ServerStartup : IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDotBPE(); // 使用AMP协议

            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<MathService>();
            });

            return services.BuildServiceProvider();
        }

        public void Configure(IAppBuilder app, IHostingEnvironment env)
        {

        }

    }
}
