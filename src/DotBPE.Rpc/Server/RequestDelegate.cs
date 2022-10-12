// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public delegate Task RequestDelegate(ISocketContext<AmpMessage> context, AmpMessage message);

    public delegate Task<RpcResult<TResponse>> ServiceMethod<in TService, in TRequest, TResponse>(TService service, TRequest request)
       where TService : IServiceActor
        where TRequest : class
        where TResponse : class;

    public delegate Task<RpcResult<TResponse>> ServiceMethodWithTimeout<in TService, in TRequest, TResponse>(TService service, TRequest request, int timeout)
        where TService : IServiceActor
        where TRequest : class
        where TResponse : class;
}