using DotBPE.BestPractice.AuditLog;
using DotBPE.Extra;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加升级服务的引用
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuditLogService(this IServiceCollection @this)
        {

            @this.AddSingleton<IAuditLoggerFormat, AuditLoggerFormat>(); //审计日志格式化插件
            @this.AddSingleton<IRequestAuditLoggerFactory, ServiceAuditLoggerFactory>(); //服务端审计日志
            @this.AddSingleton<IClientAuditLoggerFactory, ClientAuditLoggerFactory>(); //客户端审计日志

            return @this;
        }

        /// <summary>
        /// 添加动态代理和Protobuf序列化的默认依赖
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultService(this IServiceCollection @this)
        {
            @this.AddDynamicClientProxy(); // aop client
            @this.AddDynamicServiceProxy(); // aop service
            @this.AddProtobufSerializerAndJsonParser(); //protobuf 序列化和json格式化 
            return @this;
        }
    }
}
