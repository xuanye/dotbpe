using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;

namespace Survey.App
{
    public class InteropServer
    {
        public static async Task<IServerHost> StartAsync<TStartup>(string[] args) where TStartup:IStartup
        {
            //从配置文件中 读取NLog配置
            //NLoggerWrapper.InitConfig();
            //设置DotBPE内部类如何使用Logger，这里使用NLog，不配置，默认输出日志
            //DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(InteropServer)));
            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());
            // 读取json 配置        


            var config = new ConfigurationBuilder()
                         .AddJsonFile("hosting.json", optional: true)
                         .AddCommandLine(args)
                         .AddEnvironmentVariables(prefix: "DOTBPE_")
                         .Build();
            // 创建Host
            var host = new RpcHostBuilder()
                .UseConfiguration(config)
                .UseStartup<TStartup>()
                .Build();

            await host.StartAsync();
            return host;
        }
    }
}
