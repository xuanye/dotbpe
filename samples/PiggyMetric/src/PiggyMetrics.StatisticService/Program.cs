using System;
using PiggyMetrics.Common;
using Google.Protobuf;

namespace  PiggyMetrics.StatisticService
{
     class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var host =InteropServer.StartAsync<DotBpeStartup>().Result;

            Console.WriteLine("Press any key to quit!");
            Console.ReadKey();

            host.ShutdownAsync().Wait();

        }
    }
}
