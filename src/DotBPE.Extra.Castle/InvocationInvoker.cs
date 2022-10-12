// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    internal class InvocationInvoker
    {
        private readonly IInvocation _invocation;

        public InvocationInvoker(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            _invocation.Proceed();
            return (Task<TResponse>)_invocation.ReturnValue;
        }

        public Task<TResponse> HandleWithTimeout<TRequest, TResponse>(TRequest request, int timeout)
            where TRequest : class
            where TResponse : class
        {
            _invocation.Proceed();
            return (Task<TResponse>)_invocation.ReturnValue;
        }
    }
}
