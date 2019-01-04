using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotBPE.Plugin.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace PiggyMetrics.HttpApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLoggerWrapper.InitConfig();
            DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(Program)));

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
