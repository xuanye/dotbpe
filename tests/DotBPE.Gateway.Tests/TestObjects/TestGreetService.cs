// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Tests.TestObjects
{
    internal class TestService : ITestService
    {
        public Task<RpcResult<Test1Rsp>> GetAsync(Test1Req req)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<Test1Rsp>> NotHttpAsync(Test1Req req)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<Test1Rsp>> PostAsync(Test1Req req)
        {
            throw new NotImplementedException();
        }
    }

    [RpcService(101)]
    public interface ITestService
    {
        /// <summary>
        /// Test Get Method Async
        /// </summary>
        /// <param name="req">request data</param>
        /// <returns></returns>
        [RpcMethod(1)]
        [HttpRoute("/api/test/{name}", HttpVerb.Get)]
        Task<RpcResult<Test1Rsp>> GetAsync(Test1Req req);



        /// <summary>
        /// Test Post Method Async
        /// </summary>
        /// <param name="req">request data</param>
        /// <returns></returns>
        [RpcMethod(2)]
        [HttpRoute("/api/test", HttpVerb.Post)]
        Task<RpcResult<Test1Rsp>> PostAsync(Test1Req req);


        /// <summary>
        /// Not Http Api
        /// </summary>
        /// <param name="req">request data</param>
        /// <returns></returns>
        [RpcMethod(3)]
        Task<RpcResult<Test1Rsp>> NotHttpAsync(Test1Req req);




    }


    public class Test1Rsp
    {
        public string Message { get; set; }

        public List<string> ListMessages { get; set; }

    }
    public enum EmumMessage
    {
        UNSPECIFIED = 0,
        FOO = 1,
        BAR = 2,
        BAZ = 3,
        /// <summary>
        /// Intentionally negative.
        /// </summary>
        NEG = -1
    }


    /// <summary>
    /// Class sub message
    /// </summary>
    public class SubMessage
    {
        /// <summary>
        /// Sub Field1
        /// </summary>
        public string SubField1 { get; set; }


        /// <summary>
        /// Sub Fields
        /// </summary>
        public List<int> SubFields { get; set; }
    }

    public class Test1Req
    {
        /// <summary>
        /// int value
        /// </summary>
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public string StringValue { get; set; }
        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public bool BoolValue { get; set; }

        /// <summary>
        /// name comment
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sub Message
        /// </summary>
        public SubMessage SubMessage { get; set; }


        /// <summary>
        ///  enum message
        /// </summary>
        public EmumMessage EmumMessage { get; set; }
    }
}
