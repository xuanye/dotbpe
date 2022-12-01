// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway.Internal
{

    public interface IApiMethod
    {
        string ServiceName { get; }
        string Name { get; }
        string FullName { get; }
    }

    internal class ApiMethod<TRequest, TResponse> : IApiMethod
           where TRequest : class
           where TResponse : class
    {
        public ApiMethod(string serviceName, MethodInfo handler)
        {
            ServiceName = serviceName;
            HandlerMethod = handler;
            Name = handler.Name;
            FullName = GetFullName(serviceName, Name);
        }

        public string ServiceName { get; private set; }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public MethodInfo HandlerMethod { get; private set; }

        private static string GetFullName(string serviceName, string methodName)
        {
            return $"{serviceName}.{methodName}";
        }
    }

}
