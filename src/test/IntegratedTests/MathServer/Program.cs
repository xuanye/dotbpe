using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using MathCommon;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DotBPE.Hosting;
using Microsoft.Extensions.Configuration;

namespace MathServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentEnv = System.Environment.GetEnvironmentVariable("DOTBPE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("serilog.json")
                .AddJsonFile($"serilog.{currentEnv}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();


            string ip = "127.0.0.1";
            int port = 6201;
            var builder = new HostBuilder()
            .UseServer(ip, port)
            .ConfigureServices(services =>
            {
                //添加协议支持
                services.AddDotBPE();

                services.AddSingleton<IAuditLoggerFormat<AmpMessage>, AuditLoggerFormat>();

                //注册服务
                services.AddServiceActors<AmpMessage>((actors) =>
                {
                    actors.Add<MathService>();
                });
                //添加挂载的宿主服务
                services.AddScoped<IHostedService, RpcHostedService>();
            })
            .ConfigureLogging(
                logger => logger.AddSerilog(dispose: true)
            );
           



            builder.RunConsoleAsync().Wait();

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

}
