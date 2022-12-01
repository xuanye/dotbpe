// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Collections.Generic;
using RequestDelegate = Microsoft.AspNetCore.Http.RequestDelegate;

namespace DotBPE.Gateway.Internal
{
    public class ApiMethodProviderContext<TService> where TService : class
    {
        public ApiMethodProviderContext()
        {
            Methods = new List<ApiMethodModel>();
        }

        internal List<ApiMethodModel> Methods { get; }



        internal void AddMethod<TRequest, TResponse>(ApiMethod<TRequest, TResponse> method, RoutePattern pattern, IList<object> metadata, RequestDelegate invoker)
            where TRequest : class
            where TResponse : class
        {
            var methodModel = new ApiMethodModel(method, pattern, metadata, invoker);
            Methods.Add(methodModel);
        }
    }
}
