using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void ServiceDependencyRegistry(IConfiguration configuration, IServiceCollection services, Type registryType,params string[] categories)
        {
            var registry = (IServiceDependencyRegistry)Activator.CreateInstance(registryType);
            registry.AddServiceDependency(configuration, services,categories);
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
                        RpcServiceAttribute attr = null;
                        foreach (var interfaceType in t.GetInterfaces())
                        {
                            attr = interfaceType.GetCustomAttribute<RpcServiceAttribute>();
                            if (attr != null)
                            {
                                break;
                            }
                        }

                        if (attr == null) return;

                        if (EnumerableExtensions.IndexOf(categories, attr.GroupName) >= 0)
                        {
                            services.AddSingleton(serviceType, t);
                        }
                    }

                });
        }

    }
}
