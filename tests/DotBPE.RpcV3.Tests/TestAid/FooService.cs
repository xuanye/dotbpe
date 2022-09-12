// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Core;
using DotBPE.Rpc.Protocols;
using System.Threading.Tasks;


namespace DotBPE.Rpc.Tests
{
    public class FooService : BaseService<IFooService>, IFooService
    {
        public Task<RpcResult<FooRes>> FooAsync(FooReq req)
        {
            return Task.FromResult(new RpcResult<FooRes> { Data = new FooRes { RetFooWord = req.FooWord } });
        }
    }

    public class FooReq
    {
        public string FooWord {
            get; set;
        }
    }
    public class FooRes
    {
        public string RetFooWord {
            get; set;
        }
    }
}
