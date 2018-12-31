using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Client;
using DotBPE.Extra;
using Castle.DynamicProxy;

namespace MathService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
             .UseCastleDynamicProxy()
             .UseMessagePack()
             .ConfigureServices((context, services) =>
             {
                 services.AddSingleton<IServiceActor<AmpMessage>, Definition.MathService>();
                 services.AddSingleton<IServiceActor<AmpMessage>, Definition.FooService>();
             })
             .UseRpcServer()
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
