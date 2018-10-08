using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotBPE.Protobuf
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static IServiceCollection ScanAddDescriptorFactory(this IServiceCollection services, string dllPrefix = "",string pluginDirName="")
        {
            if (services.Any(x => x.ServiceType == typeof(IProtobufDescriptorFactory)))
            {
                //存在则不重复注册
                return services;
            }

            var dllFiles = Directory.GetFiles(string.Concat(BaseDirectory, pluginDirName), $"{dllPrefix}*.dll");

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            DefaultProtobufDescriptorFactory descriptorFactory = new DefaultProtobufDescriptorFactory();
            //检测类型
            BindingFlags flag = BindingFlags.Static | BindingFlags.Public;

            foreach (Assembly a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (Type type in a.GetTypes())
                {

                    var property = type.GetProperties(flag).Where(t => t.Name == "Descriptor").FirstOrDefault();
                    if (property is null)
                        continue;
                    var fileDescriptor = property.GetValue(null) as FileDescriptor;
                    if (fileDescriptor is null)
                        continue;

                    AddRpcServices(descriptorFactory,  fileDescriptor.Services);
                }
            }           
            return services.AddSingleton<IProtobufDescriptorFactory>(descriptorFactory);
        }

        /// <summary>
        /// 添加默认审计日志，使用该功能需要提前注册ScanAddDescriptorFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuditLog(this IServiceCollection services)
        {
            return services.AddSingleton<IAuditLoggerFormat<AmpMessage>, AuditLoggerFormat>();
        }

        /// <summary>
        /// 添加消息协议转换，使用该功能需要提前注册ScanAddDescriptorFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageParser(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageParser<AmpMessage>, MessageParser>();
        }

        private static void AddRpcServices(DefaultProtobufDescriptorFactory descriptorFactory, IList<Google.Protobuf.Reflection.ServiceDescriptor> serviceDescriptors)
        {
            foreach (var service in serviceDescriptors)
            {
                if (service.CustomOptions.TryGetInt32(DotBPEOptionConstant.OPTION_SERVICE_ID, out var serviceId))
                {
                    AddRpcMotheds(descriptorFactory, serviceId, service.Methods);
                }
            }
        }

        private static void AddRpcMotheds(DefaultProtobufDescriptorFactory descriptorFactory, int serviceId,  IList<MethodDescriptor> methodDescriptors)
        {
            foreach (var method in methodDescriptors)
            {
                if (method.CustomOptions.TryGetInt32(DotBPEOptionConstant.OPTION_MESSAGE_ID, out var messageId))
                {
                    //注册请求参数类型
                    descriptorFactory.AddInputType(serviceId, messageId, method.InputType);
                    //注册响应参数类型
                    descriptorFactory.AddOutputType(serviceId, messageId, method.OutputType);
                }
            }
        }



    }
}
