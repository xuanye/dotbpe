// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Rpc.Core
{
    public class Method : IMethod
    {
        public Method(string groupName, string serviceName, string methodName, MethodInfo handler, int serviceId, int methodId)
        {
            GroupName = groupName;
            ServiceName = serviceName;
            MethodName = methodName;
            ServiceId = serviceId;
            MethodId = methodId;
            Handler = handler;
        }

        public string GroupName {
            get;
        }
        public string ServiceName {
            get;
        }
        public string MethodName {
            get;
        }
        public int ServiceId {
            get;
        }
        public int MethodId {
            get;
        }

        public string FullName => $"{ServiceName}.{MethodName}";


        public MethodInfo Handler {
            get;
        }

    }
}
