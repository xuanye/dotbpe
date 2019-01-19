using System;
using PiggyMetrics.Common;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging.Console;
namespace PiggyMetrics.AuthService
{
    class Program
    {
        static void Main(string[] args)
        {
           
            var host = InteropServer.StartAsync<DotBpeStartup>().Result;

            Console.WriteLine("Press any key to quit!");
            Console.ReadKey();

            host.ShutdownAsync().Wait();
        }
    }
}
