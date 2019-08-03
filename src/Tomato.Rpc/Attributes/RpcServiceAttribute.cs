using System;
using System.Collections.Generic;
using System.Text;

namespace Tomato.Rpc
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceAttribute : Attribute
    {
        public RpcServiceAttribute()
        {

        }
        public RpcServiceAttribute(int serviceId,string groupName="default")
        {
            ServiceId = serviceId;
            GroupName = groupName;
        }

        public int ServiceId { get; set; }

        public string GroupName { get; set; } = "default";
    }
}
