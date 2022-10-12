﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Extra.Castle.Tests
{
    public class ServiceActorInterceptorTests
    {
        [Fact]
        public async Task ServiceInterceptor_Should_RecieveMessage()
        {
            var routerMock = new Mock<IServiceRouter>();

            IRouterPoint local = new RouterPoint
            {
                RoutePointType = RoutePointType.Local
            };
            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(local));

            var invokerMock = new Mock<ICallInvoker>();

            var services = new ServiceCollection();
            services.AddLogging();
            services.BindService<FooService>();
            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);

            FooReq recReq = null;
            RpcResult<FooRes> recRes = null;
            services.AddServiceInterceptor(new LogInterceptor((req, res) =>
            {
                recReq = req as FooReq;
                recRes = res as RpcResult<FooRes>;
            }));

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            var client = await proxy.CreateAsync<IFooService>();

            Assert.NotNull(client);

            var message = "Hello DotBPE!";
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(message, res.Data.RetFooWord);

            Assert.NotNull(recReq);
            Assert.Equal(message, recReq.FooWord);

            Assert.NotNull(recRes);
            Assert.NotNull(recRes.Data);
            Assert.Equal(message, recRes.Data.RetFooWord);
        }


        [Fact]
        public async Task ServiceInterceptor_ShouldNot_RecieveMessage_WhenCallRemote()
        {
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


            bool executed = false;
            services.AddServiceInterceptor(new LogInterceptor((req, res) =>
            {
                executed = true;
            }));

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            var client = await proxy.CreateAsync<IFooService>();

            Assert.NotNull(client);

            var message = "Hello DotBPE!";
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(responseMsg, res.Data.RetFooWord);

            Assert.False(executed);


        }


    }
}
