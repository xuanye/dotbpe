using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Extra
{
    public class InvokeMeta
    {
        public ushort ServiceId { get; set; }
        public ushort MessageId { get; set; }

        public bool WithNoResponse { get; set; }

        public Type ResultType { get; set; }

        public MethodInfo InvokeMethod { get; set; }
    }
}
