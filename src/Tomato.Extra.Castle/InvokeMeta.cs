using System;
using System.Reflection;
using Tomato.Rpc.Protocol;
using Tomato.Rpc.Server;

namespace Tomato.Extra
{
    public class InvokeMeta
    {
        public int ServiceId { get; set; }
        public ushort MessageId { get; set; }

        public string ServiceGroupName { get; set; }
        public bool WithNoResponse { get; set; }
        public Type ResultType { get; set; }
        public MethodInfo InvokeMethod { get; set; }

    }
}
