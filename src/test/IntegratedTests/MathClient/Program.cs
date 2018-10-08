using DotBPE.Protobuf;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using MathCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MathClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException; ;

            var currentEnv = System.Environment.GetEnvironmentVariable("DOTBPE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("serilog.json")
                .AddJsonFile($"serilog.{currentEnv}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();

            Task.Run(RunClient).Wait();
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

        public async static Task RunClient()
        {
            var proxy = new ClientProxyBuilder().UseServer("127.0.0.1:6201", 5)
                .ConfigureServices(services =>
               {
                   services.AddSingleton<IProtobufDescriptorFactory, ProtobufDescriptorFactory>();

                   services.AddSingleton<IAuditLoggerFormat<AmpMessage>, AuditLoggerFormat>();
               })
                .ConfigureLoggingServices(logger => logger.AddSerilog(dispose: true))
                .BuildDefault();

            using (var client = proxy.GetClient<MathCommon.MathClient>())
            {
                Console.WriteLine("ready to send message");

                var random = new Random();
                var i = 0;
                while (i < 10000)
                {
                    AddReq req = new AddReq
                    {
                        A = random.Next(1, 10000),
                        B = random.Next(1, 10000)
                    };

                    try
                    {
                        var t1 = client.AddAsync(req);
                        var t2 = client.AddAsync(req);
                        var t3 = client.AddAsync(req);
                        var t4 = client.AddAsync(req);
                        var t5 = client.AddAsync(req);
                        var t6 = client.AddAsync(req);
                        var t7 = client.AddAsync(req);
                        await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7);
                        Console.WriteLine("{0}+{1}={2}", req.A, req.B, t5.Result.Code == 0 ? t5.Result.Data.C : -1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error occ {0}", ex.Message);
                    }

                    i++;
                }
            }
        }
    }
}
