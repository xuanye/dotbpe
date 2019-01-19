using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotBPE.Plugin.Logging;
using DotBPE.Rpc;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;

namespace PiggyMetrics.Common
{
    public class InteropServer
    {
        public static async Task<IServerHost> StartAsync<TStartup>() where TStartup:IStartup
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //从配置文件中 读取NLog配置
            NLoggerWrapper.InitConfig();
            //设置DotBPE内部类如何使用Logger，这里使用NLog，不配置，默认输出日志
            DotBPE.Rpc.Environment.SetLogger(new NLoggerWrapper(typeof(InteropServer)));

            // 读取json 配置
            var builder = new ConfigurationBuilder().AddJsonFile("dotbpe.config.json");
            var configuration = builder.Build();



            // 创建Host
            var host = new RpcHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<TStartup>()
                .Build();

            await host.StartAsync();
            return host;
        }
    }
}
