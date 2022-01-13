using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotBPE.Gateway
{
    public class Method<TRequest, TResponse> : IMethod
    {
        public Method(string serviceName, MethodInfo handler)
        {
            this.ServiceName = serviceName;
            this.HandlerMethod = handler;
            this.Name = handler.Name;
            this.FullName = GetFullName(serviceName, this.Name);
        }

        public string ServiceName { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }


        public MethodInfo HandlerMethod { get; set; }

        //
        // Summary:
        //     Gets full name of the method including the service name.
        internal static string GetFullName(string serviceName, string methodName)
        {
            return "/" + serviceName + "/" + methodName;
        }
    }
}
