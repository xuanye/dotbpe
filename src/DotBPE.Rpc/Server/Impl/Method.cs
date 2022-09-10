using System;
using System.Reflection;

namespace DotBPE.Rpc.Server
{
    public class Method<TRequest, TResponse> : IMethod
    {
        public Method(string serviceName, string methodName, int serviceId, int methodId,MethodInfo handlerMethod)
        {
            ServiceId = serviceId;
            ServiceName = serviceName;
            MethodName = methodName;
            MethodId = methodId;
            HandlerMethod = handlerMethod;
        }

        public Type RequestType => typeof(TRequest);
        public Type ResponseType => typeof(TResponse);

        public string ServiceName { get; }
        public string MethodName { get;  }
        public int ServiceId { get;  }
        public int MethodId { get;  }

        public string FullName => $"{ServiceName}.{MethodName}";
        public string Key => $"{ServiceId}.{MethodId}";
        public MethodInfo HandlerMethod { get; }
    }
}
