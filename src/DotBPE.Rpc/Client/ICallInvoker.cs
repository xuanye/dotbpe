// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{

    public interface ICallInvoker : IMessageSubscriber
    {
        Task<RpcResult<TResponse>> InvokerAsync<TRequest, TResponse>(IMethod method, TRequest request)
            where TRequest : class
            where TResponse : class;
    }
}
