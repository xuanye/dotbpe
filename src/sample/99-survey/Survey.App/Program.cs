using DotBPE.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Survey.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //配置Dapper的映射方式
            //设置dapper在查询映射字符串时支持user_id -> UserId
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            // 创建Host ，这里可以进一步简化
            var host = new HostBuilder()
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
                .ConfigureLogging((context, factory) =>
                {
                    factory.AddConfiguration(context.Configuration.GetSection("Logging"));
                    factory.AddConsole();                  
                })
                .ConfigureServices(ServerStartup.ConfigureServices);

            host.RunConsoleAsync().Wait();
            Console.WriteLine("服务已退出");
        }
    }
}
