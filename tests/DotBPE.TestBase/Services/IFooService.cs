// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using DotBPE.Rpc.Server;
using System.Threading.Tasks;

namespace DotBPE.TestBase
{
    [RpcService(100, GroupName = "test")]
    public interface IFooService : IServiceActor
    {
        [RpcMethod(1)]
        Task<RpcResult<FooRes>> FooAsync(FooReq req);
    }
}
