using System;

namespace Survey.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = InteropServer.StartAsync<ServerStartup>(args).Result;

            Console.WriteLine("Press any key to quit!");
            Console.ReadKey();

            host.ShutdownAsync().Wait();
        }
    }
}
