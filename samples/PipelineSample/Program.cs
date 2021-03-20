using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace PipelineSample
{
    static class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        static IWebHost BuildWebHost(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                 .UseUrls("http://*:5560") //HTTP绑定在5560端口            
                 .UseStartup<Startup>()              
                 .Build();
    }

}

