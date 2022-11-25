// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Collections.Generic;

namespace DotBPE.Gateway.Internal
{
    internal class ApiMethodModel
    {
        public ApiMethodModel(IApiMethod method, RoutePattern pattern, IList<object> metadata, RequestDelegate requestDelegate)
        {
            Method = method;
            Pattern = pattern;
            Metadata = metadata;
            RequestDelegate = requestDelegate;
        }

        public IApiMethod Method { get; }
        public RoutePattern Pattern { get; }
        public IList<object> Metadata { get; }
        public RequestDelegate RequestDelegate { get; }
    }
}
