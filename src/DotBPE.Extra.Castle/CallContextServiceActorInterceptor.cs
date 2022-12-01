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

        protected override async Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, InvocationContext context, ServiceMethod<TRequest, TResponse> continuation)
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
                return await base.ServiceHandle(req, context, continuation);
            }
            finally
            {
                _contextAccessor?.CallContext.CloseDef();
            }
        }

    }
}
