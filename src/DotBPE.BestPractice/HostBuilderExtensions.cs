// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Extra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DotBPE.BestPractice
{
    public static class HostBuilderExtensions
    {

        /// <summary>
        /// 组装默认的DotBPE RPC服务端
        /// </summary>
        /// <param name="this">IHostBuilder 实例</param>
        /// <param name="port">服务绑定的端口</param>
        /// <param name="auditLog">是否启用审计日志</param>
        /// <returns></returns>
        public static IHostBuilder UseDefaultRpc(this IHostBuilder @this, int port = 5566, bool auditLog = true)
        {
            /*
            @this.UseRpcServer(port: port)
                .UseCastleDynamicProxy() //使用Castle动态代理
                .UseProtobufSerializer(true); //消息使用Protobuf序列化RPC通讯，并序列化JSON

            if (auditLog)
            {
                @this.ConfigureServices(s =>
                {  
                    s.AddAuditLogService();//审计服务              
                });
            }

            //加载默认的配置文件
            @this.ConfigureAppConfiguration((hostContext, config) =>
            {
                  config.AddJsonFile("dotbpe.json", optional: true);
                  config.AddJsonFile($"dotbpe.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);

                  config.AddJsonFile("serilog.json", optional: true);
                  config.AddJsonFile($"serilog.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            });
            */

            return @this;
        }

    }
}
