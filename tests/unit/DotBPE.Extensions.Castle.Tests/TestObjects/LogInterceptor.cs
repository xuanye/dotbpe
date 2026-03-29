// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Extensions;
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

        protected override async Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, DotBPE.Extensions.InvocationContext context, DotBPE.Extensions.ServiceMethod<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            var res = await continuation(req, context);

            _logAction(req, res);
            return res;
        }
    }
}
