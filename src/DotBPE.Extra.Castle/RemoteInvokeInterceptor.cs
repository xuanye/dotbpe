// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using System.Collections.Concurrent;
using System.Reflection;

namespace DotBPE.Extra
{
    public class RemoteInvokeInterceptor : IInterceptor
    {
        private readonly ICallInvoker _callInvoker;

        private static readonly ConcurrentDictionary<string, InvokeMeta> _metaCache =
            new ConcurrentDictionary<string, InvokeMeta>();

        private readonly MethodInfo _asyncCaller;


        public RemoteInvokeInterceptor(
            ICallInvoker callInvoker
        )
        {
            _callInvoker = callInvoker;
            _asyncCaller = callInvoker.GetType().GetMethod("InvokerAsync");
        }


        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.Name.Split('`');
            var methodFullName = $"{serviceNameArr[0]}.{invocation.Method.Name}";

            var req = invocation.Arguments[0];

            var meta = GetInvokeMeta(methodFullName, invocation);


            var arguments = meta.InvokeMethod.GetParameters();

            int timeout = invocation.Arguments.Length > 1 ?
                (int)invocation.Arguments[1] :
                arguments[1].HasDefaultValue ? (int)arguments[1].DefaultValue : 3000;

            var methodInfo = new Method()
            {
                GroupName = meta.ServiceGroupName,
                ServiceName = serviceNameArr[0],
                MethodName = invocation.Method.Name,
                ServiceId = meta.ServiceId,
                MethodId = meta.MessageId,
                DefaultTimeout = timeout
            };
            invocation.ReturnValue = meta.InvokeMethod.Invoke(_callInvoker, new[] { methodInfo, req });

        }

        private InvokeMeta GetInvokeMeta(string cacheKey, IInvocation invocation)
        {
            if (!_metaCache.TryGetValue(cacheKey, out var meta))
            {
                var service = invocation.Method.DeclaringType.GetCustomAttribute(typeof(RpcServiceAttribute), false);
                if (service == null)
                {
                    throw new RpcException("RpcServiceAttribute required");
                }
                var method = invocation.Method.GetCustomAttribute(typeof(RpcMethodAttribute), false);
                if (method == null)
                {
                    throw new RpcException("RpcMethodAttribute required");
                }
                var sAttr = service as RpcServiceAttribute;
                var mAttr = method as RpcMethodAttribute;
                meta = new InvokeMeta
                {
                    ServiceId = sAttr.ServiceId,
                    MessageId = mAttr.MessageId,
                    ServiceGroupName = sAttr.GroupName
                };

                var (requestType, responseType) = ReflectionHelper.GetInvocationCallTypes(invocation);

                meta.InvokeMethod = _asyncCaller.MakeGenericMethod(requestType, responseType);
                _metaCache.TryAdd(cacheKey, meta);
            }

            return meta;
        }
    }
}
