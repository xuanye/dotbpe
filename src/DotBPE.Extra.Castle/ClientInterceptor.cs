using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class ClientInterceptor: IInterceptor
    {
        private readonly ICallInvoker _callInvoker;

        static ConcurrentDictionary<string, InvokeMeta> META_CACHE = new ConcurrentDictionary<string, InvokeMeta>();

        private readonly MethodInfo _AsyncCaller1;
        private readonly MethodInfo _AsyncCaller2;
        public ClientInterceptor(ICallInvoker callInvoker)
        {
            _callInvoker = callInvoker;
            _AsyncCaller1 = callInvoker.GetType().GetMethod("AsyncCallWithOutResponse");
            _AsyncCaller2 = callInvoker.GetType().GetMethod("AsyncCall");
        }

        public void Intercept(IInvocation invocation)
        {
             Console.WriteLine("你正在调用方法 \"{0}\"  参数是 {1}... ",
               invocation.Method.Name,
               string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));           

            var serviceNameArr= invocation.Method.DeclaringType.FullName.Split('.');
            string cacheKey = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";
            var req = invocation.Arguments[0];

            if (!META_CACHE.TryGetValue(cacheKey,out var meta))
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
                        //resultType == null;
                        meta.WithNoResponse = true;
                    }
                    else if (innerType.BaseType == typeof(RpcResult))
                    {
                        meta.ResultType = innerType.GetProperty("Data").PropertyType;
                    }
                }
                else
                {
                    throw new DotBPE.Rpc.Exceptions.RpcException("ReturnType must be Task or Task<RpcResult<T>>");
                }
               
                if (meta.WithNoResponse)
                {                   
                    meta.InvokeMethod = this._AsyncCaller1.MakeGenericMethod(req.GetType());
                }
                else
                {
                    meta.InvokeMethod = this._AsyncCaller2.MakeGenericMethod(req.GetType(),meta.ResultType);
                }
                
                META_CACHE.TryAdd(cacheKey, meta);
            }

            if (meta.WithNoResponse)
            {
                // AsyncCallWithOutResponse<T>(string callName,ushort serviceId,ushort messageId,T req);
                invocation.ReturnValue = meta.InvokeMethod.Invoke(this._callInvoker, new object[] { cacheKey,meta.ServiceId,meta.MessageId,req });
            }
            else
            {
                //AsyncCall<T,TResult>(string callName, ushort serviceId, ushort messageId,T req, int timeOut = 3000)
                if (invocation.Arguments.Length > 1)
                {
                    invocation.ReturnValue = meta.InvokeMethod.Invoke(this._callInvoker,new object[] { cacheKey, meta.ServiceId, meta.MessageId, req,invocation.Arguments[1] } );
                }
                else
                {
                    invocation.ReturnValue = meta.InvokeMethod.Invoke(this._callInvoker,new object[] { cacheKey, meta.ServiceId, meta.MessageId, req,3000});
                }
                
            }   
            Console.WriteLine("方法执行完毕，返回结果：{0}", invocation.ReturnValue);
        }



    }
}
