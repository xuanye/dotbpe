using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;

namespace DotBPE.Extra
{
    public class ClientInterceptor : IInterceptor
    {
        private readonly ICallInvoker _callInvoker;

        private static readonly ConcurrentDictionary<string, InvokeMeta> META_CACHE = new ConcurrentDictionary<string, InvokeMeta>();
        private static readonly ConcurrentDictionary<string, bool> CACHE_LOCAL_CALL = new ConcurrentDictionary<string, bool>();

        private readonly IServiceActorLocator<AmpMessage> _actorLocator;
        private readonly IServiceRouter _serviceRouter;
        private readonly MethodInfo _AsyncCaller1;
        private readonly MethodInfo _AsyncCaller2;

        private readonly IClientAduitLogger _aduitLogger;
        public ClientInterceptor(
            ICallInvoker callInvoker,
            IServiceRouter serviceRouter,
            IServiceActorLocator<AmpMessage> actorLocator
            //IClientAduitLogger aduitLogger =null
        )
        {
            this._callInvoker = callInvoker;
            this._AsyncCaller1 = callInvoker.GetType().GetMethod("AsyncCallWithOutResponse");
            this._AsyncCaller2 = callInvoker.GetType().GetMethod("AsyncCall");

            this._actorLocator = actorLocator;
            this._serviceRouter = serviceRouter;
            //this._aduitLogger = aduitLogger?? NullClientAduitLogger.Instance;
        }


        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            string cacheKey = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            var req = invocation.Arguments[0];
            var meta = GetInvokeMeta(cacheKey, invocation);


            object ret = null;
            if (meta.IsLocal)
            {
                ret = meta.LocalActor.Invoke(meta.MessageId, invocation.Arguments);
                return;
            }
            else
            {
                if (meta.WithNoResponse)
                {
                    // AsyncCallWithOutResponse<T>(string callName,ushort serviceId,ushort messageId,T req);
                    ret = meta.InvokeMethod.Invoke(this._callInvoker,
                        new [] { cacheKey, meta.ServiceId, meta.MessageId, req });
                }
                else
                {
                    //AsyncCall<T,TResult>(string callName, ushort serviceId, ushort messageId,T req, int timeOut = 3000)
                    object timeout = invocation.Arguments.Length > 1 ? invocation.Arguments[1] : 3000;
                    ret = meta.InvokeMethod.Invoke(this._callInvoker,
                        new [] { cacheKey, meta.ServiceId, meta.MessageId, req, timeout });
                }
            }

            invocation.ReturnValue = ret;
        }

        private InvokeMeta GetInvokeMeta(string cacheKey, IInvocation invocation)
        {
            var req = invocation.Arguments[0];
            if (!META_CACHE.TryGetValue(cacheKey, out var meta))
            {
                var service = invocation.Method.DeclaringType.GetCustomAttribute(typeof(RpcServiceAttribute), false);
                if (service == null)
                {
                    throw new RpcException("RpcServiceAttribute Not Found");
                }
                var method = invocation.Method.GetCustomAttribute(typeof(RpcMethodAttribute), false);
                if (method == null)
                {
                    throw new RpcException("RpcMethodAttribute Not Found");
                }
                var sAttr = service as RpcServiceAttribute;
                var mAttr = method as RpcMethodAttribute;
                meta = new InvokeMeta { ServiceId = sAttr.ServiceId, MessageId = mAttr.MessageId };

                meta.IsLocal = IsLocalCall(meta.ServiceId, meta.MessageId);
                if (!meta.IsLocal)
                {
                    var returnType = invocation.Method.ReturnType;
                    if (returnType == typeof(Task))
                    {
                        meta.WithNoResponse = true;
                    }
                    else if (returnType.BaseType == typeof(Task)) //Task<RpcResult>
                    {
                        var innerType = returnType.GetProperty("Result").PropertyType;
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
                        throw new RpcException("ReturnType must be Task or Task<RpcResult<T>>");
                    }

                    if (meta.WithNoResponse || meta.ResultType == null)
                    {
                        meta.InvokeMethod = this._AsyncCaller1.MakeGenericMethod(req.GetType());
                    }
                    else
                    {
                        meta.InvokeMethod = this._AsyncCaller2.MakeGenericMethod(req.GetType(), meta.ResultType);
                    }
                }
                else
                {
                    meta.LocalActor = this._actorLocator.LocateServiceActor($"{meta.ServiceId}${meta.MessageId}");
                }

                META_CACHE.TryAdd(cacheKey, meta);
            }
            return meta;
        }
        private bool IsLocalCall(ushort serviceId, ushort messageId)
        {
            string key = $"{serviceId}${messageId}";
            if (CACHE_LOCAL_CALL.TryGetValue(key, out var isLocal))
            {
                return isLocal;
            }

            var point = this._serviceRouter.FindRouterPoint(key);
            isLocal = point.RoutePointType == RoutePointType.Local;
            CACHE_LOCAL_CALL.TryAdd(key, isLocal);
            return isLocal;
        }

    }
}
