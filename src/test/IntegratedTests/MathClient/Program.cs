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

        public async static Task RunClient()
        {

            var proxy = new ClientProxyBuilder().UseServer("127.0.0.1:6201")
                .ConfigureServices( services =>
                {
                    services.AddSingleton<IAuditLoggerFormat<AmpMessage>, AuditLoggerFormat>();
                })
                .ConfigureLoggingServices(logger=> logger.AddSerilog(dispose: true))
                .BuildDefault();

           
           
            using (var client = proxy.GetClient<MathCommon.MathClient>())
            {
                Console.WriteLine("ready to send message");

                var random = new Random();
                var i = 0;
                while( i < 1000)
                {
                    AddReq req = new AddReq
                    {
                        A = random.Next(1, 10000),
                        B = random.Next(1, 10000)
                    };

                    Console.WriteLine("call sever MathService.Add  --> {0}+{1} ", req.A, req.B);

                    try
                    {
                        var res = await client.AddAsync(req);
                        Console.WriteLine("server repsonse:<-----{0}+{1}={2}", req.A, req.B, res.Data.C);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error occ {0}", ex.Message);
                    }

                    i++;
                }
               
            }
            Console.WriteLine("Press any key to quit!");
            Console.ReadKey();
           

        }
    }
}
