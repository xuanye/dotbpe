// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcMethodAttribute : Attribute
    {
        public RpcMethodAttribute()
        {

        }
        public RpcMethodAttribute(ushort messageId)
        {
            MessageId = messageId;
        }

        public ushort MessageId { get; set; }

    }


}
