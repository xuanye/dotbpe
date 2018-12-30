using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Client;

namespace MathService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
             .UseRpcServer()            
             .ConfigureServices((context, services) =>
             {
                 services.AddSingleton<ISerializer, DotBPE.Extra.MessagePackSerializer>();
                 services.AddSingleton<IServiceActor<AmpMessage>, Definition.MathService>();
             })
             .ConfigureLogging(
                 logger =>
                 {
                     logger.AddConsole();
                 }
             );

            builder.RunConsoleAsync().Wait();
        }
    }
}
