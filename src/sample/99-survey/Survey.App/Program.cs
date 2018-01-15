using System;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Hosting;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Survey.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //从配置文件中 读取NLog配置
            //NLoggerWrapper.InitConfig();
            //设置DotBPE内部类如何使用Logger，这里使用NLog，不配置，默认输出日志
            //DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(InteropServer)));
            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());

            //配置Dapper的映射方式
            //设置dapper在查询映射字符串时支持user_id -> UserId
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            // 创建Host
            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddJsonFile("hosting.json", optional: true)
                   .AddCommandLine(args)
                   .AddEnvironmentVariables(prefix: "DOTBPE_");
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("dotbpe.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"dotbpe.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables(prefix: "DOTBPE_");
                   
                })          
                .ConfigureServices(ServerStartup.ConfigureServices); //配置启动的服务



            hostBuilder.RunConsoleAsync().Wait();
            Console.WriteLine("服务已退出");

        }
    }
}
