﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Server;
using DotBPE.TestBase;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.AutoMock;
using System;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class DefaultServiceActorHandlerFactoryTests
    {
        [Fact]
        public void RegisterActorInvoker_NoError()
        {
            //arrange
            var autoMocker = new AutoMocker();
            var factory = autoMocker.CreateInstance<DefaultServiceActorHandlerFactory>();
            //var locator = autoMocker.GetMock<IServiceActorLocator>();

            var methodInfo = typeof(IFooService).GetMethod("FooAsync");
            //"default", "FooService", "FooAsync", methodInfo, 100, 1
            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync",
            };
            var actorModel = new ActorInvokerModel(method, null);

            //act
            factory.RegisterActorInvokerHandler(actorModel);

        }

        [Fact]
        public void GetInstance_ReturnCorrectHandler_CorrectMethodId()
        {
            //arrange
            var autoMocker = new AutoMocker();
            var factory = autoMocker.CreateInstance<DefaultServiceActorHandlerFactory>();
            var locator = autoMocker.GetMock<IServiceActorLocator>();

            var serializer = new Mock<ISerializer>();


            var methodInfo = typeof(IFooService).GetMethod("FooAsync");
            var serviceMethod = (ServiceMethod<IFooService, FooReq, FooRes>)Delegate.CreateDelegate(typeof(ServiceMethod<IFooService, FooReq, FooRes>), methodInfo);

            var fooService = new FooService();
            var invoker = new MethodInvoker<IFooService, FooReq, FooRes>(serviceMethod, null, 0);
            var callHandler = new ActorCallHandler<IFooService, FooReq, FooRes>(locator.Object, invoker, serializer.Object, NullLoggerFactory.Instance);
            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync"
            };
            var actorModel = new ActorInvokerModel(method, callHandler.HandleCallAsync);

            var methodIdentity = $"{method.ServiceId}.{method.MethodId}";
            locator.Setup(x => x.LocateServiceActor(methodIdentity)).Returns(fooService);

            //act
            factory.RegisterActorInvokerHandler(actorModel);

            var actorHandler = factory.GetInstance(methodIdentity);

            Assert.NotNull(actorHandler);





        }
        [Fact]
        public void GetInstance_ReturnNotFoundHandler_IncorrectMethodId()
        {
            //arrange
            var autoMocker = new AutoMocker();
            var factory = autoMocker.CreateInstance<DefaultServiceActorHandlerFactory>();
            var locator = autoMocker.GetMock<IServiceActorLocator>();

            var serializer = new Mock<ISerializer>();


            var methodInfo = typeof(IFooService).GetMethod("FooAsync");
            var serviceMethod = (ServiceMethod<IFooService, FooReq, FooRes>)Delegate.CreateDelegate(typeof(ServiceMethod<IFooService, FooReq, FooRes>), methodInfo);

            var fooService = new FooService();
            var invoker = new MethodInvoker<IFooService, FooReq, FooRes>(serviceMethod, null, 0);
            var callHandler = new ActorCallHandler<IFooService, FooReq, FooRes>(locator.Object, invoker, serializer.Object, NullLoggerFactory.Instance);
            var method = new Method()
            {
                GroupName = "default",
                ServiceName = "FooService",
                ServiceId = 100,
                MethodId = 1,
                MethodName = "FooAsync"
            };
            var actorModel = new ActorInvokerModel(method, callHandler.HandleCallAsync);

            var methodIdentity = $"{method.ServiceId}.{method.MethodId}";
            locator.Setup(x => x.LocateServiceActor(methodIdentity)).Returns(fooService);

            //act
            factory.RegisterActorInvokerHandler(actorModel);


            var actorNotFoundHandler = factory.GetInstance($"101.1");
            Assert.Equal(NotFoundServiceActorHandler.Instance, actorNotFoundHandler);

        }
    }
}
