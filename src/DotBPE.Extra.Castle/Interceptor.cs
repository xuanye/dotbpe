// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public abstract class Interceptor : IInterceptor
    {

        private const string _handleMethodName = "HandleIt";

        private readonly MethodInfo _methodCache;

        public Interceptor()
        {
            _methodCache = typeof(Interceptor).GetMethod(_handleMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Intercept(IInvocation invocation)
        {
            var (requestType, responseType) = ReflectionHelper.GetInvocationCallTypes(invocation);

            var invoker = _methodCache.MakeGenericMethod(requestType, responseType);

            invoker.Invoke(this, new[] { invocation.Arguments[0], invocation });
        }


        private Task<RpcResult<TResponse>> HandleIt<TRequest, TResponse>(TRequest request, IInvocation invocation)
            where TRequest : class
            where TResponse : class
        {
            var methodName = $"{invocation.Method.DeclaringType.Name}.{invocation.Method.Name}";
            var returnType = invocation.Method.ReturnType;

            var callContext = new InvocationContext() { Method = invocation.Method };

            if (invocation.Arguments.Length > 1 && invocation.Arguments[1].GetType() == typeof(int))
            {
                callContext.Timeout = (int)invocation.Arguments[1];
            }

            var serviceMethod = new ServiceMethod<TRequest, TResponse>(async (req, context) =>
            {
                invocation.Proceed();
                var result = invocation.ReturnValue;
                if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType) //Task<RpcResult>
                {
                    var resultTask = result as Task;
                    await resultTask;
                    var property = result.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                    if (property == null)
                        throw new InvalidOperationException("Task does not have a return value (" + result.GetType().ToString() + ")");
                    var response = property.GetValue(result);
                    return (RpcResult<TResponse>)response;
                }
                else
                {
                    throw new RpcException("Return type must be Task or Task<RpcResult<T>>");
                }

            });

            return ServiceHandle(request, callContext, serviceMethod);

        }


        protected virtual Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, InvocationContext callContext, ServiceMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(req, callContext);
        }


    }
}
