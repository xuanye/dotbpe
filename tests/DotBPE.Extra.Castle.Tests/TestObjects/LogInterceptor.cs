// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Extra.Castle.Tests.TestObjects
{
    public class LogInterceptor : ClientInterceptor
    {
        private readonly Action<object, object> _logAction;

        public LogInterceptor(Action<object, object> logAction)
        {
            _logAction = logAction;
        }

        protected override async Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, InvocationContext context, ServiceMethod<TRequest, TResponse> continuation)
        {
            var res = await continuation(req, context);

            _logAction(req, res);
            return res;
        }
    }
}
