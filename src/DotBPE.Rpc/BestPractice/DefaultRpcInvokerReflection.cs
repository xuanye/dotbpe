using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DotBPE.Rpc.Client;

namespace DotBPE.Rpc.BestPractice
{
    public class DefaultRpcInvokerReflection:IRpcInvokerReflection
    {
        private readonly ConcurrentDictionary<int, Type> SERVICE_CACHE = new ConcurrentDictionary<int, Type>();
        private readonly ConcurrentDictionary<string, IRpcInvoker> INVOKER_CACHE = new ConcurrentDictionary<string, IRpcInvoker>();

        private readonly MethodInfo _proxyCreate;
        private readonly IClientProxy _proxy;

        public DefaultRpcInvokerReflection(IClientProxy clientProxy)
        {
            this._proxy = clientProxy;
            this._proxyCreate = clientProxy.GetType().GetMethod("Create");
        }

        public void Scan(string dllPrefix,Func<Type,RpcServiceAttribute,bool> filter = null)
        {
            string basePath = Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(basePath, ""), $"{dllPrefix}.dll");

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            foreach (var a in assemblies)
            {

                foreach (var type in a.GetTypes())
                {

                    if (!type.IsInterface)
                        continue;

                    var sAttr = type.GetCustomAttribute<RpcServiceAttribute>();
                    if (sAttr == null)
                        continue;

                    if (filter != null && filter(type,sAttr))
                    {
                        SERVICE_CACHE.TryAdd(sAttr.ServiceId, type);
                    }
                    else
                    {
                        SERVICE_CACHE.TryAdd(sAttr.ServiceId, type);
                    }
                }
            }
        }

        public IRpcInvoker GetRpcInvoker(int serviceId, ushort messageId)
        {
            var key = $"{serviceId}|{messageId}";
            if (this.INVOKER_CACHE.TryGetValue(key, out var invoker))
            {
                return invoker;
            }

            return GetInvokeMethod(serviceId, messageId);
        }

        private IRpcInvoker GetInvokeMethod(int serviceId,ushort messageId)
        {
            var key = $"{serviceId}|{messageId}";
            if(this.INVOKER_CACHE.TryGetValue(key,out var cacheInstance))
            {
                return cacheInstance;
            }

            Type serviceType = FindInvokeServiceType(serviceId);
            var rpcMethod = new DefaultRpcInvoker
            {
                ServiceId = serviceId,
                MessageId = messageId,
                ServiceType = serviceType ?? throw new Exception($"{serviceId} 对应的服务定义不存在")
            };

            var args = new object[] {ushort.MinValue};
            rpcMethod.ServiceInstance = this._proxyCreate.MakeGenericMethod(serviceType).Invoke(this._proxy, args);
            rpcMethod.InvokeMethod = FindInvokeMethod(rpcMethod.ServiceType, messageId);

            this.INVOKER_CACHE.TryAdd(key, rpcMethod);
            return rpcMethod;
        }

        private Type FindInvokeServiceType(int serviceId)
        {
            if (this.SERVICE_CACHE.TryGetValue(serviceId, out var serviceType))
            {
                return serviceType;
            }

            return null;
        }

        private MethodInfo FindInvokeMethod(Type invokeServiceType, ushort messageId)
        {
            var methods = invokeServiceType.GetMethods();
            foreach (var m in methods)
            {
                var mAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (mAttr == null)
                    continue;

                var rAttr = m.GetCustomAttribute<RpcMethodAttribute>();
                if (rAttr == null)
                    continue;

                if(rAttr.MessageId == messageId)
                {
                    return m;
                }
            }
            throw new Exception($"{invokeServiceType} 中 不包含 MessageId = {messageId}的方法定义");
        }
    }
}
