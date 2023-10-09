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
    public class ServiceActorInterceptorTests
    {
        [Fact]
        public async Task ServiceInterceptor_RecieveMessage_AfterRegistered()
        {
            //arrange
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
            var message = "Hello DotBPE!";

            //act
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
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
        public async Task ServiceInterceptor_DoesNotRecieveMessage_CallRemote()
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


            bool executed = false;
            services.AddServiceInterceptor(new LogInterceptor((req, res) =>
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
            Assert.Equal(responseMsg, res.Data.RetFooWord);
            Assert.False(executed);


        }



        [Fact]
        public async Task ServiceInterceptor_ShouldBeSuccess_BreakCallNextAndHandleProcessing()
        {
            //arrange
            var routerMock = new Mock<IServiceRouter>();


            IRouterPoint local = new RouterPoint
            {
                RoutePointType = RoutePointType.Local
            };

            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(local));
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

            string handleMsg = "HandleMessage from DotBPE";
            bool executed = false;
            services.AddServiceInterceptor(new BreakInterceptor(() =>
            {
                executed = true;
                return new RpcResult<object>() { Code = 100, Data = new FooRes() { RetFooWord = handleMsg } };
            }));

            var provider = services.BuildServiceProvider();
            var proxy = provider.GetService<IClientProxy>();
            var client = await proxy.CreateAsync<IFooService>();


            var message = "Hello DotBPE!";
            //act
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
            Assert.NotNull(res);
            Assert.Equal(100, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(handleMsg, res.Data.RetFooWord);
            Assert.True(executed);


        }




    }

    public class BreakInterceptor : Interceptor
    {
        private readonly Func<RpcResult<object>> _handler;

        public BreakInterceptor(Func<RpcResult<object>> handler)
        {
            _handler = handler;
        }
        protected override Task<RpcResult<TResponse>> ServiceHandle<TRequest, TResponse>(TRequest req, InvocationContext callContext, ServiceMethod<TRequest, TResponse> continuation)
        {
            var handlerResult = _handler();

            var result = new RpcResult<TResponse>()
            {
                Code = handlerResult.Code,
                Data = handlerResult.Data as TResponse
            };

            return Task.FromResult(result);

        }
    }
}
