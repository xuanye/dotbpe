using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    [AttributeUsage(AttributeTargets.Interface , AllowMultiple = false, Inherited = true)]
    public class RpcServiceAttribute : Attribute
    {
        public RpcServiceAttribute()
        {

        }
        public RpcServiceAttribute(ushort serviceId)
        {
            this.ServiceId = serviceId;
        }

        public ushort ServiceId { get; set; }
    }
}
