// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public delegate Task RequestDelegate(IServiceActor serviceActor, ISocketContext<AmpMessage> context, AmpMessage message)
           ;

    public delegate Task<RpcResult<TResponse>> ServiceMethod<TService, in TRequest, TResponse>(IServiceActor service, TRequest? request)
        where TService : class
        where TRequest : class
        where TResponse : class;

    public delegate Task<RpcResult<TResponse>> ServiceMethodWithTimeout<TService, in TRequest, TResponse>(IServiceActor service, TRequest? request, int timeout)
        where TService : class
        where TRequest : class
        where TResponse : class;
}