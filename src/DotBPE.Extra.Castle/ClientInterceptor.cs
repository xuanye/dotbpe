using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBPE.Extra.Castle
{
    public class ClientInterceptor: IInterceptor
    {
        private readonly ICallInvoker<AmpMessage> _callInvoker;

        static ConcurrentDictionary<string, InvokeMeta> META_CACHE = new ConcurrentDictionary<string, InvokeMeta>();

        public ClientInterceptor(ICallInvoker<AmpMessage> callInvoker)
        {
            _callInvoker = callInvoker;
        }

        public void Intercept(IInvocation invocation)
        {
             Console.WriteLine("你正在调用方法 \"{0}\"  参数是 {1}... ",
               invocation.Method.Name,
               string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            var serviceNameArr= invocation.Method.DeclaringType.FullName.Split('.');
            string cacheKey = $"{serviceNameArr[serviceNameArr.Length-1]}.{invocation.Method.Name}";

            if(!META_CACHE.TryGetValue(cacheKey,out var meta))
            {
                var service = invocation.Method.DeclaringType.GetCustomAttributes(typeof(RpcServiceAttribute), false).FirstOrDefault();
                if (service == null)
                {
                    throw new Rpc.Exceptions.RpcException("RpcServiceAttribute Not Found");
                }
                var method = invocation.Method.GetCustomAttributes(typeof(RpcMethodAttribute), false).FirstOrDefault();
                if (method == null)
                {
                    throw new Rpc.Exceptions.RpcException("RpcMethodAttribute Not Found");
                }

                var sAttr = service as RpcServiceAttribute;
                var mAttr = service as RpcMethodAttribute;
                meta = new InvokeMeta() { ServiceId = sAttr.ServiceId, MessageId = mAttr.MessageId };
                META_CACHE.TryAdd(cacheKey, meta);
            }

            AmpMessage message = AmpMessage.CreateRequestMessage(meta.ServiceId, meta.MessageId);
            message.FriendlyServiceName = cacheKey;
            var taskRsp = _callInvoker.AsyncCall(message);

            //taskRsp.Result.Data
            //TODO:序列化
            //message.Data = req.ToByteArray();     
            //invocation.ReturnValue 
            //在被拦截的方法执行完毕后 继续执行
            //invocation.Proceed();
            invocation.ReturnValue = invocation.Arguments[0];


            Console.WriteLine("方法执行完毕，返回结果：{0}", invocation.ReturnValue);
        }
    }
}
