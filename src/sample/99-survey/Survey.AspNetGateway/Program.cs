using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Survey.AspNetGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("hosting.json", optional: true)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                        .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
