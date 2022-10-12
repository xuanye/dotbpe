// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class CallContextServiceActorInterceptor : Interceptor
    {

        private readonly IContextAccessor _contextAccessor;

        public CallContextServiceActorInterceptor(IContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, ServiceMethod<TRequest, TResponse> continuation)
        {
            if (_contextAccessor != null)
            {
                if (_contextAccessor.CallContext == null)
                {
                    _contextAccessor.CallContext = new CallContext();
                }
                _contextAccessor.CallContext.AddDef();
            }

            try
            {
                return await base.ServiceHandle(req, continuation);
            }
            finally
            {
                _contextAccessor?.CallContext.CloseDef();
            }
        }
        protected override async Task<RpcResult<TResponse>> ServiceHandleWithTimeout<TRequest, TResponse>(TRequest req, int timeout, ServiceMethodWithTimeout<TRequest, TResponse> continuation)
        {
            if (_contextAccessor != null)
            {
                if (_contextAccessor.CallContext == null)
                {
                    _contextAccessor.CallContext = new CallContext();
                }
                _contextAccessor.CallContext.AddDef();
            }

            try
            {
                return await base.ServiceHandleWithTimeout(req, timeout, continuation);
            }
            finally
            {
                _contextAccessor?.CallContext.CloseDef();
            }

        }
    }
}
