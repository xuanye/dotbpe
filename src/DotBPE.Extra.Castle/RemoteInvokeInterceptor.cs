using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class RemoteInvokeInterceptor : IInterceptor
    {
        private readonly ICallInvoker _callInvoker;

        private static readonly ConcurrentDictionary<string, InvokeMeta> META_CACHE =
            new ConcurrentDictionary<string, InvokeMeta>();

        private readonly MethodInfo _AsyncCaller1;
        private readonly MethodInfo _AsyncCaller2;
        private readonly IClientAuditLoggerFactory _auditLoggerFactory;


        public RemoteInvokeInterceptor(
            ICallInvoker callInvoker,
            IClientAuditLoggerFactory auditLogFactory
        )
        {
            this._callInvoker = callInvoker;
            this._AsyncCaller1 = callInvoker.GetType().GetMethod("AsyncNotify");
            this._AsyncCaller2 = callInvoker.GetType().GetMethod("AsyncRequest");
            this._auditLoggerFactory = auditLogFactory;
        }


        public void Intercept(IInvocation invocation)
        {
            var serviceNameArr = invocation.Method.DeclaringType.FullName.Split('.');
            string cacheKey = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";
            var req = invocation.Arguments[0];


            using (var logger = this._auditLoggerFactory.GetLogger(cacheKey))
            {
                logger.SetParameter(invocation.Arguments[0]);

                var meta = GetInvokeMeta(cacheKey, invocation);

                object ret;

                if (meta.WithNoResponse)
                {
                    // AsyncCallWithOutResponse<T>(string callName,ushort serviceId,ushort messageId,T req);
                    ret = meta.InvokeMethod.Invoke(this._callInvoker, 
                        new [] { cacheKey,meta.ServiceGroupName, meta.ServiceId, meta.MessageId, req });
                }
                else
                {
                    //AsyncCall<T,TResult>(string callName, ushort serviceId, ushort messageId,T req, int timeOut = 3000)
                    var arguments = meta.InvokeMethod.GetParameters();
                   
                    object timeout = invocation.Arguments.Length > 1 ?
                        invocation.Arguments[1] :
                        arguments[1].HasDefaultValue ? arguments[1].DefaultValue : 3000;
                   
                    ret = meta.InvokeMethod.Invoke(this._callInvoker,
                        new [] { cacheKey,meta.ServiceGroupName, meta.ServiceId, meta.MessageId, req, timeout });
                }

                invocation.ReturnValue = ret;

                logger.SetReturnValue(invocation.ReturnValue);
            }


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
                meta = new InvokeMeta
                {
                    ServiceId = sAttr.ServiceId, MessageId = mAttr.MessageId,ServiceGroupName = sAttr.GroupName
                };

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

                META_CACHE.TryAdd(cacheKey, meta);
            }

            return meta;
        }
    }
}
