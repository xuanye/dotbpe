// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    /// <summary>
    /// Server-side handler for unary call.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    public delegate Task<RpcResult<TResponse>> ServiceMethod<TRequest, TResponse>(TRequest request)
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// Server-side handler for unary call.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>   
    public delegate Task<RpcResult<TResponse>> ServiceMethodWithTimeout<TRequest, TResponse>(TRequest request, int timeout)
        where TRequest : class
        where TResponse : class;

}
