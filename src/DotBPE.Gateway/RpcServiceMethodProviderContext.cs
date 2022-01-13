using DotBPE.Gateway.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway
{
    public class RpcServiceMethodProviderContext<TService> where TService : class
    {     

        public RpcServiceMethodProviderContext()
        {
            Methods = new List<MethodModel>();           
        }

        internal List<MethodModel> Methods { get; }


        /// <summary>
        /// Adds a method to a service. This method is handled by the specified <see cref="RequestDelegate"/>.
        /// This overload of <c>AddMethod</c> is intended for advanced scenarios where raw processing of HTTP requests
        /// is desired.
        /// Note: experimental API that can change or be removed without any prior notice.
        /// </summary>
        /// <typeparam name="TRequest">Request message type for this method.</typeparam>
        /// <typeparam name="TResponse">Response message type for this method.</typeparam>
        /// <param name="method">The method description.</param>
        /// <param name="pattern">The method pattern. This pattern is used by routing to match the method to an HTTP request.</param>
        /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a RPC method.</param>
        /// <param name="invoker">The <see cref="RequestDelegate"/> that is executed when the method is called.</param>
        public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, RoutePattern pattern, IList<object> metadata, RequestDelegate invoker)
            where TRequest : class
            where TResponse : class
        {
            var methodModel = new MethodModel(method, pattern, metadata, invoker);
            Methods.Add(methodModel);
        }
    }
}
