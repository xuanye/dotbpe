// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach.Messaging;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{

    public interface ICallInvoker
    {
        Task<RpcResult<TResponse>> InvokerAsync<TRequest, TResponse>(IMethod method, TRequest request)
            where TRequest : class
            where TResponse : class;

    }
}
