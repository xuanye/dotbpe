using System;
using System.Reflection;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;

namespace DotBPE.Extra
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
