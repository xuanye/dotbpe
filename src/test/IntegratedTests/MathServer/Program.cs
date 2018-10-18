using DotBPE.Protobuf;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Options;
using DotNetty.Common.Internal.Logging;
using Flurl.Http;
using MathCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MathServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            //InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));


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
            int port = args.Length>0?6202:6201;
            var builder = new HostBuilder()
            .UseServer(ip, port)
            .ConfigureServices(services =>
            {
              
                //添加协议支持
                services.AddDotBPE();

                services.AddSingleton<IProtobufDescriptorFactory, ProtobufDescriptorFactory>();
                services.AddSingleton<IAuditLoggerFormat<AmpMessage>, DotBPE.Protobuf.AuditLoggerFormat>();

                services.Configure<RemoteServicesOption>( x=>
                {
                    x.Add(new ServiceOption()
                    {
                         ServiceId = 10006,
                         MessageId = "0",
                         RemoteAddress="127.0.0.1:6202"
                    });
                });
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

        public MathService(IClientProxy proxy)
        {
            _proxy = proxy;
        }

        public override async Task<RpcResult<AddRes>> AddAsync(AddReq req)
        {
            var inner = _proxy.GetClient<MathInnerClient>();

            var res = await inner.PlusAsync(req);

            return res;
        }
    }

    public class MathInnerService : MathInnerBase
    {
        public override Task<RpcResult<AddRes>> PlusAsync(AddReq req)
        {
            var res = new AddRes();
            res.C = req.A + req.B;
            var result = new RpcResult<AddRes>() { Data = res };

            return Task.FromResult(result);
        }
    }
}
