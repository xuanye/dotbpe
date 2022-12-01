// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Extra.Castle.Tests
{
    public class DynamicClientProxyTests
    {
        [Fact]
        public async Task Create_ReturnLocalProxy_LocalPoint()
        {
            //arrange
            var routerMock = new Mock<IServiceRouter>();

            IRouterPoint localPoint = new RouterPoint { RoutePointType = RoutePointType.Local };
            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(localPoint));

            var invokerMock = new Mock<ICallInvoker>();

            var services = new ServiceCollection();
            services.AddLogging();

            services.BindService<FooService>();

            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            //act
            var client = proxy.Create<IFooService>();
            var message = "Hello DotBPE!";
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(message, res.Data.RetFooWord);
        }

        [Fact]
        public async Task CreateAsync_ReturnLocalProxy_LocalPoint()
        {
            //arrange
            var routerMock = new Mock<IServiceRouter>();

            IRouterPoint localPoint = new RouterPoint { RoutePointType = RoutePointType.Local };
            routerMock.Setup(x => x.FindRouterPoint(It.IsAny<string>())).Returns(Task.FromResult(localPoint));

            var invokerMock = new Mock<ICallInvoker>();

            var services = new ServiceCollection();
            services.AddLogging();
            services.BindService<FooService>();
            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();

            //act
            var client = await proxy.CreateAsync<IFooService>();
            var message = "Hello DotBPE!";
            var res = await client.FooAsync(new FooReq() { FooWord = message });

            //assert
            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(message, res.Data.RetFooWord);
        }

        [Fact]
        public async Task CreateAsync_ReturnRemoteProxy_RemotePoint()
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

            IMethod recievedMethod = null;
            FooReq recievedReq = null;
            string responseMsg = "Mock from DotBPE";
            var invokerMock = new Mock<ICallInvoker>();
            invokerMock
                .Setup(x => x.InvokerAsync<FooReq, FooRes>(It.IsAny<IMethod>(), It.IsAny<FooReq>()))
                .ReturnsAsync((IMethod m, FooReq req) =>
                {
                    recievedMethod = m;
                    recievedReq = req;

                    return
                        new RpcResult<FooRes>()
                        {
                            Data = new FooRes()
                            {
                                RetFooWord = responseMsg
                            }
                        };
                });

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDynamicProxy();
            services.AddSingleton(routerMock.Object);
            services.AddSingleton(invokerMock.Object);

            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IClientProxy>();


            //act
            var client = await proxy.CreateAsync<IFooService>();
            var message = "Hello DotBPE!";
            var res = await client.FooAsync(new FooReq() { FooWord = message });


            //assert
            var serviceType = typeof(IFooService);
            var serviceAttr = serviceType.GetCustomAttribute<RpcServiceAttribute>();
            Assert.NotNull(serviceAttr);
            var methodInfo = serviceType.GetMethod("FooAsync");
            Assert.NotNull(methodInfo);

            var methodAttr = methodInfo.GetCustomAttribute<RpcMethodAttribute>();
            Assert.NotNull(methodAttr);

            Assert.NotNull(recievedMethod);
            Assert.NotNull(recievedReq);

            Assert.Equal(serviceAttr.ServiceId, recievedMethod.ServiceId);
            Assert.Equal(serviceAttr.GroupName, recievedMethod.GroupName);
            Assert.Equal(methodAttr.MessageId, recievedMethod.MethodId);

            Assert.NotNull(res);
            Assert.Equal(0, res.Code);
            Assert.NotNull(res.Data);
            Assert.Equal(responseMsg, res.Data.RetFooWord);
        }
    }
}
