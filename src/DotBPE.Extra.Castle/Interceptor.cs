// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using System;
using System.Diagnostics;
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
            var returnType = invocation.Method.ReturnType;

            if (invocation.Arguments.Length == 1)
            {
                var serviceMethod = new ServiceMethod<TRequest, TResponse>(async req =>
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

                return ServiceHandle(request, serviceMethod);
            }
            else
            {
                var serviceMethod = new ServiceMethodWithTimeout<TRequest, TResponse>(async (_1, _2) =>
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
                int timeout = (int)invocation.Arguments[1];
                return ServiceHandleWithTimeout(request, timeout, serviceMethod);
            }

        }

        protected virtual Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, ServiceMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            return continuation(req);
        }

        protected virtual Task<RpcResult<TResponse>> ServiceHandleWithTimeout<TRequest, TResponse>(TRequest req, int timeout, ServiceMethodWithTimeout<TRequest, TResponse> continuation)
          where TRequest : class
          where TResponse : class
        {
            return continuation(req, timeout);
        }

    }
}
