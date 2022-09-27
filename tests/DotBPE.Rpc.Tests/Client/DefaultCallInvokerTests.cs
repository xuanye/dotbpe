// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultCallInvokerTests
    {
        [Fact]
        public async Task TestCall_WillBeTimeout_AfterDefaultSetting3s()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var callInvoker = autoMocker.CreateInstance<DefaultCallInvoker>();
            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync",
                DefaultTimeout = 0
            };

            var reqMsg = new FooReq() { FooWord = "Hello DotBPE" };

            //act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var rspMsg = await callInvoker.InvokerAsync<FooReq, FooRes>(method, reqMsg);
            stopwatch.Stop();
            //assert
            Assert.NotNull(rspMsg);
            Assert.Equal(RpcStatusCodes.CODE_TIMEOUT, rspMsg.Code);

            Assert.InRange(stopwatch.ElapsedMilliseconds, 3000, 3100);
        }

        [Fact]
        public async Task TestCall_WillBeTimeout_After5S()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var callInvoker = autoMocker.CreateInstance<DefaultCallInvoker>();
            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync",
                DefaultTimeout = 5000
            };

            var reqMsg = new FooReq() { FooWord = "Hello DotBPE" };

            //act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var rspMsg = await callInvoker.InvokerAsync<FooReq, FooRes>(method, reqMsg);
            stopwatch.Stop();

            //assert
            Assert.NotNull(rspMsg);
            Assert.Equal(RpcStatusCodes.CODE_TIMEOUT, rspMsg.Code);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 5000, 5100);
        }

        [Fact]
        public Task TestCall_NormalProcess_ShouldBe_Ok()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var callInvoker = autoMocker.CreateInstance<DefaultCallInvoker>();
            var rpcClient = autoMocker.GetMock<IRpcClient>();

            AmpMessage receivedMsg = null;
            rpcClient.Setup(x => x.SendAsync(It.IsAny<AmpMessage>())).Callback((AmpMessage msg) =>
            {
                receivedMsg = msg;
            });

            var serializer = autoMocker.GetMock<ISerializer>();
            serializer.Setup(x => x.Serialize<FooReq>(It.IsAny<FooReq>())).Returns((FooReq src) =>
            {
                return Encoding.UTF8.GetBytes(src.FooWord);
            });
            serializer.Setup(x => x.Deserialize(It.IsAny<byte[]>(), typeof(FooReq))).Returns((byte[] data) =>
            {
                return new FooReq()
                {
                    FooWord = Encoding.UTF8.GetString(data)
                };
            });

            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync",
                DefaultTimeout = 0
            };

            var reqMsg = new FooReq() { FooWord = "Hello DotBPE" };

            //act
            var invokeTask = callInvoker.InvokerAsync<FooReq, FooRes>(method, reqMsg);

            callInvoker.Handle()
            Assert.NotNull(rspMsg);
            Assert.Equal(RpcStatusCodes.CODE_TIMEOUT, rspMsg.Code);


        }




    }
}
