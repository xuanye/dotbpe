using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotBPE.Rpc;
using DotBPE.Extra;

namespace MathService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
             .UseRpcServer()
             .UseCastleDynamicProxy()
             .UseMessagePack()
             .ConfigureServices((context, services) =>
             {
                 services.BindService<Definition.MathService>();
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
