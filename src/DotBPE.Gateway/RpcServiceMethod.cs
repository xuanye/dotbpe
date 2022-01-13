using DotBPE.Gateway.Internal;
using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public delegate Task<RpcResult<TResponse>> RpcServiceMethod<TService, TRequest, TResponse>(TService service, TRequest request);

    public delegate Task<RpcResult<TResponse>> RpcServiceMethodWithTimeout<TService, TRequest, TResponse>(TService service, TRequest request,int timeout);
}
