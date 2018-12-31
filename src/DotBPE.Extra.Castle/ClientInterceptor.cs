using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;

namespace DotBPE.Extra
{
    public class ClientInterceptor : IInterceptor
    {
        private readonly ICallInvoker _callInvoker;

        private static ConcurrentDictionary<string, InvokeMeta> META_CACHE = new ConcurrentDictionary<string, InvokeMeta>();
        private static ConcurrentDictionary<string, bool> CACHE_LOCALCALL = new ConcurrentDictionary<string, bool>();

        private readonly IServiceActorLocator<AmpMessage> _actorLocator;
        private readonly IServiceRouter _serviceRouter;
        private readonly MethodInfo _AsyncCaller1;
        private readonly MethodInfo _AsyncCaller2;
        public ClientInterceptor(
            ICallInvoker callInvoker,
            IServiceRouter serviceRouter,
            IServiceActorLocator<AmpMessage> actorLocator
        )
        {
            _callInvoker = callInvoker;
            _AsyncCaller1 = callInvoker.GetType().GetMethod("AsyncCallWithOutResponse");
            _AsyncCaller2 = callInvoker.GetType().GetMethod("AsyncCall");

            _actorLocator = actorLocator;
            _serviceRouter = serviceRouter;
        }

        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            string cacheKey = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            var req = invocation.Arguments[0];

            if (!META_CACHE.TryGetValue(cacheKey, out var meta))
            {
                var service = invocation.Method.DeclaringType.GetCustomAttributes(typeof(RpcServiceAttribute), false).FirstOrDefault();
                if (service == null)
                {
                    throw new Rpc.Exceptions.RpcException("RpcServiceAttribute Not Found");
                }
                var methods = invocation.Method.GetCustomAttributes(typeof(RpcMethodAttribute), false);
                var method = methods.FirstOrDefault();
                if (method == null)
                {
                    throw new Rpc.Exceptions.RpcException("RpcMethodAttribute Not Found");
                }
                var sAttr = service as RpcServiceAttribute;
                var mAttr = method as RpcMethodAttribute;
                meta = new InvokeMeta() { ServiceId = sAttr.ServiceId, MessageId = mAttr.MessageId };

                meta.IsLocal = IsLocalCall(meta.ServiceId, meta.MessageId);
                if (!meta.IsLocal)
                {
                    var returnType = invocation.Method.ReturnType;
                    Type innerType = null;
                    if (returnType == typeof(Task))
                    {
                        meta.WithNoResponse = true;
                    }
                    else if (returnType.BaseType == typeof(Task)) //Task<RpcResult>
                    {
                        innerType = returnType.GetProperty("Result").PropertyType;
                        if (innerType == typeof(RpcResult))
                        {
                            meta.ResultType = null;
                        }
                        else if (innerType.BaseType == typeof(RpcResult))
                        {
                            meta.ResultType = innerType.GetProperty("Data").PropertyType;
                        }
                    }
                    else
                    {
                        throw new Rpc.Exceptions.RpcException("ReturnType must be Task or Task<RpcResult<T>>");
                    }

                    if (meta.WithNoResponse || meta.ResultType == null)
                    {
                        meta.InvokeMethod = _AsyncCaller1.MakeGenericMethod(req.GetType());
                    }
                    else
                    {
                        meta.InvokeMethod = _AsyncCaller2.MakeGenericMethod(req.GetType(), meta.ResultType);
                    }
                }
                else{
                    meta.LocalActor = _actorLocator.LocateServiceActor($"{meta.ServiceId}${meta.MessageId}");
                }

                META_CACHE.TryAdd(cacheKey, meta);
            }
            if(meta.IsLocal){
                invocation.ReturnValue =  meta.LocalActor.Invoke(meta.MessageId,invocation.Arguments);
                return ;
            }
            if (meta.WithNoResponse)
            {
                // AsyncCallWithOutResponse<T>(string callName,ushort serviceId,ushort messageId,T req);
                invocation.ReturnValue = meta.InvokeMethod.Invoke(_callInvoker, new object[] { cacheKey, meta.ServiceId, meta.MessageId, req });
            }
            else
            {
                //AsyncCall<T,TResult>(string callName, ushort serviceId, ushort messageId,T req, int timeOut = 3000)
                if (invocation.Arguments.Length > 1)
                {
                    invocation.ReturnValue = meta.InvokeMethod.Invoke(_callInvoker, new object[] { cacheKey, meta.ServiceId, meta.MessageId, req, invocation.Arguments[1] });
                }
                else
                {
                    invocation.ReturnValue = meta.InvokeMethod.Invoke(_callInvoker, new object[] { cacheKey, meta.ServiceId, meta.MessageId, req, 3000 });
                }
            }          
        }

        private bool IsLocalCall(ushort serviceId, ushort messageId)
        {
            string key = $"{serviceId}${messageId}";
            if (CACHE_LOCALCALL.TryGetValue(key, out var isLocal))
            {
                return isLocal;
            }
            else
            {
                var point = _serviceRouter.FindRouterPoint(key);
                isLocal = point.RoutePointType == RoutePointType.Local;
                CACHE_LOCALCALL.TryAdd(key, isLocal);
            }
            return isLocal;
        }

    }
}
