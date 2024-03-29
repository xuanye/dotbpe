// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Extra.Castle.Tests.TestObjects;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Extra.Castle.Tests
{
    public class ClientInterceptorTest
    {
        [Fact]
        public async Task ClientInterceptor_RecieveMessage_AfterRegistered()
        {
            //arrange
            var routerMock = new Mock<IServiceRouter>();
            var remoteIP = "192.168.1.1";
            var remotePort = 4001;
            IRouterPoint remotePoint = new RouterPoint
            {
                RoutePointType = RoutePointType.Remote,
                RemoteAddress = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort)
            };
            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(remotePoint));
            string responseMsg = "Mock from DotBPE";
            var invokerMock = new Mock<ICallInvoker>();
            invokerMock
                .Setup(x => x.InvokerAsync<FooReq, FooRes>(It.IsAny<IMethod>(), It.IsAny<FooReq>()))
                .Returns((IMethod m, FooReq req) =>
                {
                    return Task.FromResult(
                        new RpcResult<FooRes>()
                        {
                            Data = new FooRes()
                            {
                                RetFooWord = responseMsg
                            }
                        });
                });


            var services = new ServiceCollection();
            services.AddLogging();
            services.BindService<FooService>();
            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);

            FooReq recReq = null;
            RpcResult<FooRes> recRes = null;
            services.AddClientInterceptor(new LogInterceptor((req, res) =>
            {
                recReq = req as FooReq;
                recRes = res as RpcResult<FooRes>;
            }));

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            var client = await proxy.CreateAsync<IFooService>();
            var message = "Hello DotBPE!";

            //act
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(responseMsg, res.Data.RetFooWord);

            Assert.NotNull(recReq);
            Assert.Equal(message, recReq.FooWord);

            Assert.NotNull(recRes);
            Assert.NotNull(recRes.Data);
            Assert.Equal(responseMsg, recRes.Data.RetFooWord);
        }

        [Fact]
        public async Task ClientInterceptor_DoesNotRecieveMessage_CallLocal()
        {
            //arrange
            var routerMock = new Mock<IServiceRouter>();
            IRouterPoint localPoint = new RouterPoint
            {
                RoutePointType = RoutePointType.Local,
            };
            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(localPoint));

            var invokerMock = new Mock<ICallInvoker>();

            var services = new ServiceCollection();
            services.AddLogging();
            services.BindService<FooService>();
            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);


            bool executed = false;
            services.AddClientInterceptor(new LogInterceptor((req, res) =>
            {
                executed = true;
            }));

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            var client = await proxy.CreateAsync<IFooService>();

            var message = "Hello DotBPE!";
            //act
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(message, res.Data.RetFooWord);

            Assert.False(executed);


        }

    }

}
