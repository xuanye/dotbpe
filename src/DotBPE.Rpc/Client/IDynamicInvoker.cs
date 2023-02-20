// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface IDynamicInvoker
    {
        Task<RpcResult<string>> InvokeAsync(string json);
    }


}
