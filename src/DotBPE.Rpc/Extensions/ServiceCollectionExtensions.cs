// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Server;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection BindService<TService>(this IServiceCollection @this)
            where TService : IServiceActor
        {

            return @this.BindService(typeof(TService));
        }

        public static IServiceCollection BindService(this IServiceCollection @this, Type serviceType)
        {
            return @this.AddSingleton(typeof(IServiceActor), serviceType);
        }

        public static IServiceCollection BindServices(this IServiceCollection @this, Assembly assembly, Func<ServiceModel, bool> filterFunc = null)
        {
            var actorType = typeof(IServiceActor);
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAssignableFrom(actorType) && type.IsClass)
                {

                    var serviceAttr = type.GetCustomAttribute<RpcServiceAttribute>(true);
                    if (serviceAttr == null)
                    {
                        continue;
                    }
                    var serviceModel = new ServiceModel()
                    {
                        ServiceId = serviceAttr.ServiceId,
                        Group = serviceAttr.GroupName,
                        ServiceType = type
                    };
                    bool add = true;
                    if (filterFunc != null)
                    {
                        add = filterFunc(serviceModel);
                    }
                    if (add)
                    {
                        @this.BindService(type);
                    }
                }
            }
            return @this;
        }
    }
}
