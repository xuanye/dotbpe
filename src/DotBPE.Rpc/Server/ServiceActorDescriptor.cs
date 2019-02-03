using System;
using System.Collections.Generic;
using System.Reflection;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnumerableExtensions = DotBPE.Baseline.Extensions.EnumerableExtensions;

namespace DotBPE.Rpc
{
    public static class ServiceActorDescriptor
    {
        private static readonly Type serviceType = typeof(IServiceActor<AmpMessage>);
        public static void ServiceDependencyRegistry(IConfiguration configuration, IServiceCollection services, Type registryType)
        {
            var registry = (IServiceDependencyRegistry)Activator.CreateInstance(registryType);
            registry.AddServiceDependency(configuration, services);
        }

        public static void AddServiceActor(IServiceCollection services, List<Type> actorTypes,params string[] categories )
        {
            actorTypes.ForEach(
                t =>
                {
                    if ( categories == null || categories.Length == 0)
                    {
                        services.AddSingleton(serviceType, t);
                    }
                    else
                    {
                        var tAttr = t.GetCustomAttribute(typeof(RpcServiceAttribute), true);
                        if (tAttr == null)
                        {
                            return;
                        }

                        var rpcOption = tAttr as RpcServiceAttribute;
                        if (EnumerableExtensions.IndexOf(categories, rpcOption.GroupName) >= 0)
                        {
                            services.AddSingleton(serviceType, t);
                        }
                    }                   

                });
        }
    }
}
