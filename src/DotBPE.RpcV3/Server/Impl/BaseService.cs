// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Rpc.Attributes;
using System;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    public class BaseService<TService> : IServiceActor<TService>
        where TService : class
    {
        protected BaseService()
        {
            var serviceType = typeof(TService);
            var serviceAttribute = GetServiceAttribute(serviceType);
            ServiceId = serviceAttribute.ServiceId;
            GroupName = serviceAttribute.GroupName;
        }

        protected int ServiceId
        {
            get;
        }
        public string Id => $"{ServiceId}.0";

        public string GroupName
        {
            get;
        }

        private static RpcServiceAttribute GetServiceAttribute(Type serviceType)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<RpcServiceAttribute>(false);
            if (serviceAttribute == null)
                throw new InvalidOperationException($"Miss [RpcServiceAttribute] at {serviceType}");
            return serviceAttribute;
        }

    }
}
