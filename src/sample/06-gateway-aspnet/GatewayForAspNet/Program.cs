using DotBPE.Rpc.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace GatewayForAspNet
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, config) =>
                 {
                     config.AddJsonFile("dotbpe.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"dotbpe.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                       .AddEnvironmentVariables(prefix: "DOTBPE_");
                 })
               .UseSetting(HostDefaultKey.HOSTADDRESS_KEY, "0.0.0.0:6201") //RPC服务绑定在6201端口
               .UseUrls("http://0.0.0.0:6200") //HTTP绑定在6200端口              
               .UseStartup<Startup>()
               .Build();
    }
    
}
