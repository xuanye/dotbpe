// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Server;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Tests
{
    [RpcService(100, GroupName = "mock")]
    public interface IFooService:IServiceActor
    {
        [RpcMethod(1)]
        Task<RpcResult<FooRes>> FooAsync(FooReq req);
    }
}
