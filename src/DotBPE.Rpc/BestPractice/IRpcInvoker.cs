using System;
using System.Reflection;

namespace DotBPE.Rpc.BestPractice
{
    public interface IRpcInvoker
    {
          int ServiceId { get; }

          ushort MessageId { get; }

          Type ServiceType { get; }

          object ServiceInstance { get;  }

          MethodInfo InvokeMethod { get;  }
    }

    public class DefaultRpcInvoker:IRpcInvoker
    {
        public int ServiceId { get; set; }
        public ushort MessageId { get; set; }
        public Type ServiceType { get; set; }
        public object ServiceInstance { get; set; }
        public MethodInfo InvokeMethod { get; set; }
    }
}
