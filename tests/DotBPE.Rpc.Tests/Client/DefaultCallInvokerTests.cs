// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Tests.TestAid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        public async Task TestCall_NormalProcess_ShouldBe_Ok()
        {

            //arrange          
            var serializer = new DefaultJsonSerializer();
            var rpcClient = new Mock<IRpcClient>();
            var logger = NullLoggerFactory.Instance.CreateLogger<DefaultCallInvoker>();

            AmpMessage receivedMsg = null;
            rpcClient.Setup(x => x.SendAsync(It.IsAny<AmpMessage>())).Callback((AmpMessage msg) =>
            {
                receivedMsg = msg;
            });
            var callInvoker = new DefaultCallInvoker(rpcClient.Object, serializer, logger);

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

            var rspTask = Task.Run(async () =>
            {
                while (true)
                {
                    if (receivedMsg == null)
                    {
                        await Task.Delay(10);
                    }
                    else
                    {
                        var rspMsg = AmpMessage.CreateResponseMessage(receivedMsg);
                        rspMsg.Data = serializer.Serialize(new FooRes() { RetFooWord = "Response DotBPE" });
                        callInvoker.Handle(rspMsg);
                        break;
                    }
                }
            });

            await Task.WhenAll(invokeTask, rspTask);


            //assert
            var rspData = invokeTask.Result;

            Assert.NotNull(receivedMsg);
            Assert.Equal(100, receivedMsg.ServiceId);
            Assert.Equal(1, receivedMsg.MessageId);

            Assert.NotNull(receivedMsg.Data);
            var recoverReq = serializer.Deserialize<FooReq>(receivedMsg.Data);
            Assert.Equal("Hello DotBPE", recoverReq.FooWord);

            Assert.NotNull(rspData);
            Assert.Equal(RpcStatusCodes.CODE_SUCCESS, rspData.Code);

            Assert.NotNull(rspData.Data);
            Assert.Equal("Response DotBPE", rspData.Data.RetFooWord);

        }




    }
}
