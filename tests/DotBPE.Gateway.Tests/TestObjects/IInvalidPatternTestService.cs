// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Tests.TestObjects
{

    [RpcService(102)]
    public interface IInvalidPatternTestService
    {
        /// <summary>
        /// BadPattern
        /// </summary>
        /// <param name="req">request data</param>
        /// <returns></returns>
        [RpcMethod(4)]
        [HttpRoute("api/test/{id}")]
        Task<RpcResult<Test1Rsp>> BadPatternAsync(Test1Req req);
    }
}
