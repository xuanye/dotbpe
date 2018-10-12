using DotBPE.Rpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Protocol.Amp
{
    public static class ServiceActorDescriptor
    {
        public static void ServiceDependencyRegistry(IConfiguration configuration, IServiceCollection services, Type registryType)
        {
            IServiceDependencyRegistry registry = (IServiceDependencyRegistry)Activator.CreateInstance(registryType);
            registry.AddServiceDependency(configuration, services);
        }

        public static void AddServiceActor(IServiceCollection services, List<Type> actorTypes)
        {
            Type serviceType = typeof(IServiceActor<AmpMessage>);

            actorTypes.ForEach(
                t => services.AddSingleton(serviceType, t)
            );
        }
    }
}
