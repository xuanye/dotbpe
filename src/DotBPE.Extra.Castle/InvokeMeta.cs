using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;

namespace DotBPE.Extra
{
    public class InvokeMeta
    {
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }

        public bool WithNoResponse { get; set; }

        public Type ResultType { get; set; }

        public MethodInfo InvokeMethod { get; set; }

        public bool IsLocal{get;set;}
        public IServiceActor<AmpMessage> LocalActor {get;set;}
    }
}
