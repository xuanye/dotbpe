using DotBPE.Plugin.Logging;
using System;

namespace PiggyMetrics.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            NLoggerWrapper.InitConfig();
            DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(Program)));

            string[] runArgs = new string[] { "--server", "127.0.0.1:6203", "--testcase", "auth_create" };
            InteropClient.Run(runArgs);
        }
    }
}