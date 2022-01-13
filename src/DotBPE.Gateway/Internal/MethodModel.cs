using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Collections.Generic;

namespace DotBPE.Gateway
{
    internal class MethodModel
    {
        public MethodModel(IMethod method, RoutePattern pattern, IList<object> metadata, RequestDelegate requestDelegate)
        {
            Method = method;
            Pattern = pattern;
            Metadata = metadata;
            RequestDelegate = requestDelegate;
        }

        public IMethod Method { get; }
        public RoutePattern Pattern { get; }
        public IList<object> Metadata { get; }
        public RequestDelegate RequestDelegate { get; }
    }
}
