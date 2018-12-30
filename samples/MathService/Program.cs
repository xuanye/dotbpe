using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace MathService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
             .ConfigureServices((context, services) =>
             {
               
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
