using Microsoft.AspNetCore;
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
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   config.AddJsonFile("dotbpe.json", optional: true, reloadOnChange: true) //服务相关的配置
                    .AddJsonFile($"dotbpe.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                   config.AddCommandLine(args);
               })
               .UseStartup<Startup>()
               .Build();
    }
}
