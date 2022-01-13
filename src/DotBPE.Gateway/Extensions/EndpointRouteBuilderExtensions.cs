using DotBPE.Gateway.Internal;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Environment = DotBPE.Rpc.Internal.Environment;


namespace Microsoft.AspNetCore.Routing
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapService<TService>(this IEndpointRouteBuilder builder) where TService : class
        {
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder<TService>>();

            var loggerFactory = builder.ServiceProvider.GetRequiredService<ILoggerFactory>();
            Environment.SetServiceProvider(builder.ServiceProvider);
            Environment.SetLoggerFactory(loggerFactory);


            IEnumerable<IEndpointConventionBuilder> endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new RpcServiceEndpointConventionBuilder(endpointConventionBuilders);
        }
        private static IEndpointConventionBuilder MapService(this IEndpointRouteBuilder builder,Type serviceType)
        {
            var builderType = typeof(ServiceRouteBuilder<>);         
            var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService(builderType.MakeGenericType(serviceType)) as IServiceRouteBuilder;

            IEnumerable<IEndpointConventionBuilder> endpointConventionBuilders = serviceRouteBuilder.Build(builder);

            return new RpcServiceEndpointConventionBuilder(endpointConventionBuilders);
        }

        public static void ScanMapServices(this IEndpointRouteBuilder builder, string dllPrefix = "*", params string[] categories)
        {
            //builder.ServiceProvider 
            var loggerFactory =  builder.ServiceProvider.GetRequiredService<ILoggerFactory>();         
            Environment.SetServiceProvider(builder.ServiceProvider);
            Environment.SetLoggerFactory(loggerFactory);


            var logger=  loggerFactory.CreateLogger("RouteBuilder");

            string basePath = DotBPE.Rpc.Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(basePath, ""), $"{dllPrefix}.dll");

            logger.LogInformation("dll count={0}", dllFiles.Length);


            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {

                assemblies.Add(Assembly.LoadFrom(file));
            }

            logger.LogInformation("assembly count={0}", assemblies.Count);

            foreach (var a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var type in a.GetTypes())
                {

                    if (!type.IsInterface)
                        continue;

                    //this._logger.LogInformation(type.FullName);
                    var sAttr = type.GetCustomAttribute<RpcServiceAttribute>();
                    if (sAttr == null)
                        continue;

                    if ( categories.Any() && categories.Contains(sAttr.GroupName)
                        || "default".Equals(sAttr.GroupName, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        builder.MapService(type);
                    }
                   
                }
            }
        }

 

    }
}
