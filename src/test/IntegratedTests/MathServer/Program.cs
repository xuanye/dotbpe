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
using Microsoft.Extensions.Caching.Distributed;
using DotBPE.Protobuf;
using Flurl.Http;

namespace MathServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

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
                //services.AddDistributedRedisCache(options =>
                //{
                //    options.Configuration = "10.240.225.136:6379";
                //    options.InstanceName = "Survey:";
                //});

                //添加协议支持
                services.AddDotBPE();

                services.AddSingleton<IProtobufObjectFactory, ProtobufObjectFactory>();
                services.AddSingleton<IAuditLoggerFormat<AmpMessage>, DotBPE.Protobuf.AuditLoggerFormat>();

                //注册服务
                services.AddServiceActors<AmpMessage>((actors) =>
                {
                    actors.Add<MathService>();
                    actors.Add<MathInnerService>();
                });




                //添加挂载的宿主服务
                services.AddScoped<IHostedService, RpcHostedService>();
            })
            .ConfigureLogging(
                logger => logger.AddSerilog(dispose: true)
            );

            builder.RunConsoleAsync().Wait();

        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Exception.ToString());
            }
            catch
            {

            }
        }
    }


    public class MathService : MathBase
    {

        private readonly IClientProxy _proxy;

        public MathService( IClientProxy proxy)
        {
            _proxy = proxy;

        }
        public override async Task<RpcResult<AddRes>> AddAsync(AddReq req){
            var inner = _proxy.GetClient<MathInnerClient>();

            var res = await inner.PlusAsync(req);

            return res;
        }
    }

    public class MathInnerService : MathInnerBase
    {
        private readonly ClientReq _req;
        public MathInnerService()
        {
            _req = new ClientReq();
        }

        public override async Task<RpcResult<AddRes>> PlusAsync(AddReq req)
        {
            await Task.Delay(3000);

            string response =  await _req.GetRequest();

            Console.WriteLine("response:{0}", response);
            var res = new AddRes();
            res.C = req.A + req.B;
            return new RpcResult<AddRes>() { Data = res };
        }
    }


    public class ClientReq {

        public async Task<string> GetRequest()
        {
            string url = "https://www.mocky.io/v2/5185415ba171ea3a00704eed";
            return await url.PostStringAsync("").ReceiveString();
        }

    }


}
